Blazor.registerCustomEventType('bchfiledrop', {
    browserEventName: 'drop',
    createEventArgs: event => {
        event.preventDefault();

        const files = event.dataTransfer.files;
        if (!files.length) return { files: [] }

        const dropZone = event.target?.closest('[bch-file-drop-zone]');
        const createImagePreview = dropZone?.hasAttribute('create-image-preview');
        const fileInfos = [];

        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            const guid = bchGetUUID();

            console.log(file.type);

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

        return { files: fileInfos };
    }
});

Blazor.registerCustomEventType('bchdropdragover', {
    browserEventName: 'dragover',
    createEventArgs: event => {
        event.preventDefault();
        return {}
    }
});

Blazor.registerCustomEventType('bchdropdragleave', {
    browserEventName: 'dragleave',
    createEventArgs: event => {
        return {}
    }
});