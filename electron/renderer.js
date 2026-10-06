const file1Input = document.getElementById('file1-input');
const file2Input = document.getElementById('file2-input');
const compareBtn = document.getElementById('compare-btn');
const resultSection = document.getElementById('result-section');
const saveReportBtn = document.getElementById('save-report-btn');
const tableBody = document.getElementById('table-body');
const toast = document.getElementById('toast');

const countChanged = document.getElementById('count-changed');
const countOnly1 = document.getElementById('count-only1');
const countOnly2 = document.getElementById('count-only2');

let bom1Path = null;
let bom2Path = null;
let lastResult = null;
let currentTab = 'changed';

// ---------- 工具函数 ----------
function setFilePath(el, path) {
  el.classList.add('has-file');
  el.textContent = path;
}

function clearFilePath(el) {
  el.classList.remove('has-file');
  const span = document.createElement('span');
  span.className = 'placeholder';
  span.textContent = '尚未选择文件';
  el.innerHTML = '';
  el.appendChild(span);
}

let toastTimer = null;
function showToast(message, isError = false) {
  toast.textContent = message;
  toast.classList.toggle('error', isError);
  toast.hidden = false;
  clearTimeout(toastTimer);
  toastTimer = setTimeout(() => {
    toast.hidden = true;
  }, 3000);
}

function escapeHtml(str) {
  return String(str)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');
}

// ---------- 选择文件 ----------
document.querySelectorAll('.btn-select').forEach((btn) => {
  btn.addEventListener('click', async () => {
    const target = btn.dataset.target;
    try {
      const filePath = await window.api.selectFile(target);
      if (!filePath) return;
      if (target === '1') {
        bom1Path = filePath;
        setFilePath(file1Input, filePath);
      } else {
        bom2Path = filePath;
        setFilePath(file2Input, filePath);
      }
    } catch (err) {
      showToast(`选择文件失败: ${err.message}`, true);
    }
  });
});

// ---------- 开始对比 ----------
compareBtn.addEventListener('click', async () => {
  if (!bom1Path || !bom2Path) {
    showToast('请先选择两个 BOM 文件！', true);
    return;
  }

  compareBtn.disabled = true;
  compareBtn.textContent = '对比中...';

  try {
    const result = await window.api.compare(bom1Path, bom2Path);

    if (result.error) {
      showToast(`对比失败: ${result.error}`, true);
      return;
    }

    lastResult = result;
    countChanged.textContent = result.partChanges.length;
    countOnly1.textContent = result.onlyInBom1.length;
    countOnly2.textContent = result.onlyInBom2.length;
    saveReportBtn.hidden = false;

    resultSection.hidden = false;
    renderTable();

    showToast(`对比完成，共 ${result.partChanges.length + result.onlyInBom1.length + result.onlyInBom2.length} 处差异`);
  } catch (err) {
    showToast(`对比失败: ${err.message}`, true);
  } finally {
    compareBtn.disabled = false;
    compareBtn.textContent = '开始对比';
  }
});

// ---------- 标签页切换 ----------
document.querySelectorAll('.tab').forEach((tab) => {
  tab.addEventListener('click', () => {
    document.querySelectorAll('.tab').forEach((t) => t.classList.remove('active'));
    tab.classList.add('active');
    currentTab = tab.dataset.tab;
    renderTable();
  });
});

// ---------- 渲染表格 ----------
function renderTable() {
  if (!lastResult) return;
  tableBody.innerHTML = '';

  let items = [];
  if (currentTab === 'changed') items = lastResult.partChanges;
  else if (currentTab === 'only1') items = lastResult.onlyInBom1;
  else items = lastResult.onlyInBom2;

  if (items.length === 0) {
    tableBody.innerHTML = `<tr><td colspan="4" class="empty-row">(None) — 无差异</td></tr>`;
    return;
  }

  const frag = document.createDocumentFragment();

  items.forEach((item, idx) => {
    const tr = document.createElement('tr');

    const idxTd = document.createElement('td');
    idxTd.className = 'col-idx';
    idxTd.textContent = idx + 1;

    const locTd = document.createElement('td');
    locTd.className = 'col-loc';
    locTd.textContent = item.RefDes;

    const pn1Td = document.createElement('td');
    const pn2Td = document.createElement('td');
    pn1Td.className = 'pn-cell';
    pn2Td.className = 'pn-cell';

    const maxRows = Math.max(item.PnList1.length, item.PnList2.length, 1);
    for (let r = 0; r < maxRows; r++) {
      const p1 = r < item.PnList1.length ? item.PnList1[r] : '';
      const p2 = r < item.PnList2.length ? item.PnList2[r] : '';
      if (p1 || r === 0) pn1Td.innerHTML += `<span class="pn-item">${escapeHtml(p1)}</span>`;
      if (p2 || r === 0) pn2Td.innerHTML += `<span class="pn-item">${escapeHtml(p2)}</span>`;
    }

    tr.appendChild(idxTd);
    tr.appendChild(locTd);
    tr.appendChild(pn1Td);
    tr.appendChild(pn2Td);
    frag.appendChild(tr);
  });

  tableBody.appendChild(frag);
}

// ---------- 保存报告 ----------
saveReportBtn.addEventListener('click', async () => {
  if (!lastResult) return;
  try {
    const res = await window.api.saveReport(lastResult);
    if (res.canceled) return;
    if (res.error) {
      showToast(`保存失败: ${res.error}`, true);
      return;
    }
    showToast(`报告已保存到: ${res.savedPath}`);
  } catch (err) {
    showToast(`保存失败: ${err.message}`, true);
  }
});
