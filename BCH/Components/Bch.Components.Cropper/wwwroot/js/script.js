
function bchOnCropImage(canvasId, canvasHolderId, imageId, pos, imgBounds, angle,
                                 scale, resultFormat, quality, backgroundColor, imgPixelRatio, rectDisplacement, rectSize, fileName) {
    return new Promise((resolve) => {
        try {
            const canvas = document.getElementById(canvasId);
            const img = document.getElementById(imageId);
            if (!canvas || !img) { resolve(null); return; }

            const ctx = canvas.getContext('2d');
            if (!ctx) { resolve(null); return; }

            const dpi = window.devicePixelRatio;
            let scl = imgPixelRatio * dpi;
            const canvasHolderRect = {
                width: rectSize.x,
                height: rectSize.y
            };
            if (canvasHolderRect.width <= 0 || canvasHolderRect.height <= 0) { resolve(null); return; }

            canvas.width = canvasHolderRect.width * scl;
            canvas.height = canvasHolderRect.height * scl;

            ctx.resetTransform();
            ctx.clearRect(0, 0, canvasHolderRect.width * scl, canvasHolderRect.height * scl);
            ctx.fillStyle = backgroundColor || '#00000000';
            ctx.fillRect(0, 0, canvasHolderRect.width * scl, canvasHolderRect.height * scl);

            ctx.save();
            ctx.translate((pos.x - rectDisplacement.x) * scl, (pos.y - rectDisplacement.y) * scl);
            ctx.rotate(angle || 0);
            ctx.save();
            ctx.drawImage(img, 0, 0, imgBounds.x * scale * scl, imgBounds.y * scale * scl);

            
            const type = resultFormat || 'image/png';
            const q = typeof quality === 'number' ? quality : 0.92;

            canvas.toBlob((blob) => {
                try {
                    if (!blob) { resolve(null); return; }
                    const ext = (type.split('/')[1] || 'png');
                    const file = new File([blob], `${fileName || 'cropped-image'}.${ext}`, { type: type, lastModified: Date.now() });

                    const id = bchGetUUID();
                    bchPickedFilesStatic[id] = file;

                    resolve({
                        id: id,
                        lastModified: new Date(file.lastModified).toISOString(),
                        name: file.name,
                        size: file.size,
                        contentType: file.type,
                        imagePreviewUrl: URL.createObjectURL(file)
                    });
                } catch {
                    resolve(null);
                }
            }, type, q);
        } catch {
            resolve(null);
        }
    });
}