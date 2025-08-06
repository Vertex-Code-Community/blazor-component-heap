
function bchOnCropImage(canvasId, canvasHolderId, imageId, pos, imgBounds, angle, 
                        scale, resultFormat, quality, backgroundColor, croppedWidth, rectDisplacement, rectSize) {

    // const canvasHolder = document.getElementById(canvasHolderId);
    const canvas = document.getElementById(canvasId);
    const img = document.getElementById(imageId);
    const ctx = canvas.getContext('2d');

    let scl = 1.0;
   // const canvasHolderRect = canvasHolder.getBoundingClientRect();

    const canvasHolderRect = {
        width: rectSize.x,
        height: rectSize.y
    };
    
    if (croppedWidth > 0) {
        scl = croppedWidth / canvasHolderRect.width;
    }
    
    canvas.width = canvasHolderRect.width * scl;
    canvas.height = canvasHolderRect.height * scl;
    
    ctx.resetTransform();
    ctx.clearRect(0, 0, canvasHolderRect.width * scl, canvasHolderRect.height * scl);
    ctx.fillStyle = backgroundColor;
    ctx.fillRect(0, 0, canvasHolderRect.width * scl, canvasHolderRect.height * scl);
    
    ctx.save();
    ctx.translate((pos.x - rectDisplacement.x) * scl, (pos.y - rectDisplacement.y) * scl);
    ctx.rotate(angle);
    ctx.save();
    ctx.drawImage(img, 0, 0, imgBounds.x * scale * scl, imgBounds.y * scale * scl);

    // let imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
    return canvas.toDataURL(resultFormat, quality);
}