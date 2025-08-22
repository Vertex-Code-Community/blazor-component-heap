
Blazor.registerCustomEventType('bchonfilechange', {
    browserEventName: 'change',
    createEventArgs: event => {
        const files = event?.target?.files ?? [];
        const createImagePreview = event?.target?.hasAttribute('create-image-preview');

        const fileInfos = [];

        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            console.log(file);
            
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

        return {
            files: fileInfos
        };
    }
});