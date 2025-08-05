const bchPickedFilesStatic = {};

async function pickFile_BCH(multiple, createImagePreview) {
    try {
        const fileHandles = await window.showOpenFilePicker({
            multiple: multiple
        });
        
        const files = [];

        for (const handle of fileHandles) {
            const file = await handle.getFile();
            files.push(file);
        }

        const fileInfos = [];
        
        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            const guid = bchGetUUID();

            const fileInfo = {
                id: guid,
                lastModified: new Date(file.lastModified).toISOString(),
                name: file.name,
                size: file.size,
                contentType: file.type,
                imagePreviewUrl: createImagePreview && file.type.startsWith("image/") ? URL.createObjectURL(file) : null
            };

            fileInfos.push(fileInfo);
            bchPickedFilesStatic[guid] = file;
        }
        
        return fileInfos;
    } catch {
        return null;
    }
}

function readFileStream_BCH(id) {
    return bchPickedFilesStatic[id];
}

function disposeFileState_BCH(id) {
    if (!bchPickedFilesStatic[id]) return;
    delete bchPickedFilesStatic[id];
}

function bchGetUUID() {
    if (crypto?.randomUUID) {
        return crypto.randomUUID();
    }

    // Polyfill for Safari < 15.4
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        const r = crypto.getRandomValues(new Uint8Array(1))[0] & 15;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}