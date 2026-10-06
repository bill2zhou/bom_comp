const { app, BrowserWindow, dialog, ipcMain } = require('electron');
const path = require('path');
const fs = require('fs');

// ============================================================
// 与 C# util/Compare.cs 逻辑完全对应的实现
// ============================================================

// 预编译正则表达式（等价于 C# PartLineRegex / RefLineRegex）
const PartLineRegex = /^\|\s\d\s\d{4}\s(\w+)/;
const RefLineRegex = /^\|\s{10}\d{4}\s(\w+)/;

// 需要过滤的关键词 (不区分大小写)
const FilterKeywords = ['ZZZ', 'DAX', 'PCB'];

/**
 * 检查字符串是否包含需要过滤的关键词 (等价 IsFiltered)
 */
function isFiltered(text) {
  if (!text) return false;
  const upper = text.toUpperCase();
  return FilterKeywords.some((kw) => upper.indexOf(kw.toUpperCase()) >= 0);
}

/**
 * BomData 数据结构 (等价 C# BomData 类)
 */
class BomData {
  constructor() {
    // 唯一元件实例集合 (位号_料号)
    this.uniqueComponents = new Set();
    // 位号 -> 出现次数
    this.refCountMap = new Map();
    // 位号 -> 唯一料号集合
    this.refToPartsSet = new Map();
    // 位号 -> 主要料号 (最后一次出现)
    this.refToPartMap = new Map();
  }

  addComponent(refDes, partNumber) {
    const key = `${refDes}_${partNumber}`;
    this.uniqueComponents.add(key);

    if (!this.refCountMap.has(refDes)) {
      this.refCountMap.set(refDes, 0);
      this.refToPartsSet.set(refDes, new Set());
    }

    this.refCountMap.set(refDes, this.refCountMap.get(refDes) + 1);
    this.refToPartsSet.get(refDes).add(partNumber);
    this.refToPartMap.set(refDes, partNumber);
  }
}

/**
 * 解析单个 BOM 文件 (等价 ParseBom)
 */
function parseBom(filePath) {
  const data = new BomData();
  let currentPartNumber = '';

  if (!fs.existsSync(filePath)) {
    throw new Error(`BOM file not found: ${filePath}`);
  }

  const content = fs.readFileSync(filePath, 'utf8');
  const lines = content.split(/\r?\n/);

  for (const line of lines) {
    if (!line) continue;

    const partMatch = PartLineRegex.exec(line);
    if (partMatch) {
      currentPartNumber = partMatch[1];
      continue;
    }

    const refMatch = RefLineRegex.exec(line);
    if (refMatch) {
      const refDes = refMatch[1];
      if (currentPartNumber && !isFiltered(refDes)) {
        data.addComponent(refDes, currentPartNumber);
      }
    }
  }

  return data;
}

/**
 * DiffItem 数据结构 (等价 C# DiffItem)
 */
class DiffItem {
  constructor(refDes, pnList1, pnList2, type) {
    this.RefDes = refDes;
    this.PnList1 = pnList1;
    this.PnList2 = pnList2;
    this.Type = type;
  }
}

/**
 * 对齐写入两行料号 (等价 WriteAlignedRows)
 */
function writeAlignedRows(lines, index, location, pnList1, pnList2, wIdx, wLoc, wPn) {
  const maxRows = Math.max(pnList1.length, pnList2.length) || 1;

  for (let r = 0; r < maxRows; r++) {
    let idxStr = (r === 0) ? index.padEnd(wIdx) : ''.padEnd(wIdx);
    let locStr = (r === 0) ? location.padEnd(wLoc) : ''.padEnd(wLoc);

    const pn1 = (r < pnList1.length) ? pnList1[r] : '';
    const pn1Str = pn1.padEnd(wPn);

    const pn2 = (r < pnList2.length) ? pnList2[r] : '';
    const pn2Str = pn2.padEnd(wPn);

    lines.push(idxStr + locStr + pn1Str + pn2Str);
  }
}

/**
 * 对比两个 BOM (等价 comp)，只计算差异并返回结果供 UI 显示，不自动生成文件
 */
function compare(bom1, bom2) {
  // 1. 解析
  const data1 = parseBom(bom1);
  const data2 = parseBom(bom2);

  // 2. 分类差异
  const partChanges = [];
  const onlyInBom1 = [];
  const onlyInBom2 = [];

  // 获取所有位号 (并集)
  const allRefs = new Set([...data1.refToPartMap.keys(), ...data2.refToPartMap.keys()]);

  for (const refDes of allRefs) {
    const in1 = data1.refToPartMap.has(refDes);
    const in2 = data2.refToPartMap.has(refDes);

    if (in1 && in2) {
      // 两边都存在，检查料号集合是否完全一致
      const set1 = data1.refToPartsSet.get(refDes);
      const set2 = data2.refToPartsSet.get(refDes);

      const equal =
        set1.size === set2.size &&
        [...set1].every((p) => set2.has(p));

      if (!equal) {
        partChanges.push(new DiffItem(
          refDes,
          [...set1].sort((a, b) => a.localeCompare(b)),
          [...set2].sort((a, b) => a.localeCompare(b)),
          'Part Changed'
        ));
      }
    } else if (in1 && !in2) {
      onlyInBom1.push(new DiffItem(
        refDes,
        [...data1.refToPartsSet.get(refDes)].sort((a, b) => a.localeCompare(b)),
        [],
        'Only in Bom1'
      ));
    } else if (!in1 && in2) {
      onlyInBom2.push(new DiffItem(
        refDes,
        [],
        [...data2.refToPartsSet.get(refDes)].sort((a, b) => a.localeCompare(b)),
        'Only in Bom2'
      ));
    }
  }

  // 3. 排序 (不区分大小写)
  const sortFn = (a, b) => a.RefDes.localeCompare(b.RefDes, undefined, { sensitivity: 'base' });
  partChanges.sort(sortFn);
  onlyInBom1.sort(sortFn);
  onlyInBom2.sort(sortFn);

  const filename1 = path.basename(bom1, path.extname(bom1));
  const filename2 = path.basename(bom2, path.extname(bom2));

  return {
    partChanges,
    onlyInBom1,
    onlyInBom2,
    filename1,
    filename2,
  };
}

/**
 * 根据对比结果生成 txt 报告内容 (等价 comp 中的文件生成逻辑)
 */
function buildDiffText(result) {
  const filename1 = result.filename1;
  const filename2 = result.filename2;
  const partChanges = result.partChanges;
  const onlyInBom1 = result.onlyInBom1;
  const onlyInBom2 = result.onlyInBom2;

  const colIdxWidth = 4;
  const colLocWidth = 15;
  const colPnWidth = 30;

  const lines = [];

  const header =
    ''.padEnd(colIdxWidth) +
    'Location'.padEnd(colLocWidth) +
    filename1.padEnd(colPnWidth) +
    filename2.padEnd(colPnWidth);

  let i = 1;

  // --- 仅在 Bom1 ---
  lines.push(`=== Only in ${filename1} ===`);
  lines.push(header.replace(filename2.padEnd(colPnWidth), ''.padEnd(colPnWidth)));
  for (const item of onlyInBom1) {
    writeAlignedRows(lines, i.toString(), item.RefDes, item.PnList1, item.PnList2, colIdxWidth, colLocWidth, colPnWidth);
    i++;
  }
  if (onlyInBom1.length === 0) lines.push('(None)');
  lines.push('');

  // --- 仅在 Bom2 ---
  lines.push(`=== Only in ${filename2} ===`);
  lines.push(header.replace(filename1.padEnd(colPnWidth), ''.padEnd(colPnWidth)));
  for (const item of onlyInBom2) {
    writeAlignedRows(lines, i.toString(), item.RefDes, item.PnList1, item.PnList2, colIdxWidth, colLocWidth, colPnWidth);
    i++;
  }
  if (onlyInBom2.length === 0) lines.push('(None)');

  // --- 料号变更 ---
  lines.push('=== Part Number Differences (Exist in Both) ===');
  lines.push(header);
  for (const item of partChanges) {
    writeAlignedRows(lines, i.toString(), item.RefDes, item.PnList1, item.PnList2, colIdxWidth, colLocWidth, colPnWidth);
    i++;
  }
  if (partChanges.length === 0) lines.push('(None)');
  lines.push('');

  return lines.join('\n') + '\n';
}

// ============================================================
// 主进程：窗口创建 与 IPC
// ============================================================

let mainWindow = null;

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 760,
    height: 560,
    title: 'BOM 对比工具',
    resizable: true,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      nodeIntegration: false,
    },
  });

  mainWindow.loadFile(path.join(__dirname, 'index.html'));
  mainWindow.removeMenu();
}

// IPC：选择文件
ipcMain.handle('select-file', async (event, target) => {
  const result = await dialog.showOpenDialog(mainWindow, {
    title: target === 1 ? '选择 BOM 1 文件' : '选择 BOM 2 文件',
    filters: [
      { name: 'BOM Files', extensions: ['txt', 'csv', 'xls', 'xlsx'] },
      { name: 'All Files', extensions: ['*'] },
    ],
    properties: ['openFile'],
  });

  if (result.canceled || result.filePaths.length === 0) {
    return null;
  }
  return result.filePaths[0];
});

// IPC：执行对比
ipcMain.handle('compare', (event, bom1, bom2) => {
  try {
    return compare(bom1, bom2);
  } catch (err) {
    return { error: err.message };
  }
});

// IPC：保存报告文件（弹出保存对话框，让用户选择保存路径）
ipcMain.handle('save-report', async (event, result) => {
  if (!result || !result.filename1) {
    return { error: '没有可保存的对比结果' };
  }

  const defaultName = `diff_${result.filename1}_${result.filename2}.txt`;
  const dialogResult = await dialog.showSaveDialog(mainWindow, {
    title: '保存对比报告',
    defaultPath: path.join(app.getPath('documents'), defaultName),
    filters: [
      { name: 'Text Files', extensions: ['txt'] },
      { name: 'All Files', extensions: ['*'] },
    ],
  });

  if (dialogResult.canceled || !dialogResult.filePath) {
    return { canceled: true };
  }

  const content = buildDiffText(result);
  try {
    fs.writeFileSync(dialogResult.filePath, content, 'utf8');
    return { savedPath: dialogResult.filePath };
  } catch (err) {
    return { error: err.message };
  }
});

app.whenReady().then(() => {
  createWindow();

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit();
});
