using Microsoft.JSInterop;

namespace BlazorComponentHeap.Files.Streams;

public class BchBrowserFileStream : Stream
{
    private long _position;
    private readonly IJSRuntime _jsRuntime;
    private readonly BchBrowserFile _file;
    private readonly long _maxAllowedSize;
    private readonly CancellationTokenSource _openReadStreamCts;
    private readonly Task<Stream> _openReadStreamTask;
    private IJSStreamReference? _jsStreamReference;

    private bool _isDisposed;
    private CancellationTokenSource? _copyFileDataCts;

    public BchBrowserFileStream(
        IJSRuntime jsRuntime,
        BchBrowserFile file,
        long maxAllowedSize,
        CancellationToken cancellationToken)
    {
        _jsRuntime = jsRuntime;
        _file = file;
        _maxAllowedSize = maxAllowedSize;
        _openReadStreamCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _openReadStreamTask = OpenReadStreamAsync(_openReadStreamCts.Token);
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => _file.Size;

    public override long Position
    {
        get => _position;
        set => throw new NotSupportedException();
    }

    public override void Flush()
        => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count)
        => throw new NotSupportedException("Synchronous reads are not supported.");

    public override long Seek(long offset, SeekOrigin origin)
        => throw new NotSupportedException();

    public override void SetLength(long value)
        => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var bytesAvailableToRead = Length - Position;
        var maxBytesToRead = (int)Math.Min(bytesAvailableToRead, buffer.Length);
        if (maxBytesToRead <= 0) return 0;

        var bytesRead = await CopyFileDataIntoBuffer(buffer.Slice(0, maxBytesToRead), cancellationToken);
        _position += bytesRead;
        return bytesRead;
    }

    private async Task<Stream> OpenReadStreamAsync(CancellationToken cancellationToken)
    {
        // This method only gets called once, from the constructor, so we're never overwriting an
        // existing _jsStreamReference value
        _jsStreamReference = await _jsRuntime.InvokeAsync<IJSStreamReference>(
            "readFileStream_BCH",
            cancellationToken,
            _file.Id);

        return await _jsStreamReference.OpenReadStreamAsync(
            _maxAllowedSize,
            cancellationToken: cancellationToken);
    }

    private async ValueTask<int> CopyFileDataIntoBuffer(Memory<byte> destination, CancellationToken cancellationToken)
    {
        var stream = await _openReadStreamTask;
        _copyFileDataCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        return await stream.ReadAsync(destination, _copyFileDataCts.Token);
    }

    protected override void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        _openReadStreamCts.Cancel();
        _copyFileDataCts?.Cancel();

        // If the browser connection is still live, notify the JS side that it's free to release the Blob
        // and reclaim the memory. If the browser connection is already gone, there's no way for the
        // notification to get through, but we don't want to fail the .NET-side disposal process for this.
        try
        {
            _ = _jsStreamReference?.DisposeAsync().Preserve();
        }
        catch
        {
            // ignored
        }

        _isDisposed = true;

        base.Dispose(disposing);
    }
}
