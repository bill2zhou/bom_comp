const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('api', {
  selectFile: (target) => ipcRenderer.invoke('select-file', target),
  compare: (bom1, bom2) => ipcRenderer.invoke('compare', bom1, bom2),
  saveReport: (result) => ipcRenderer.invoke('save-report', result),
});
