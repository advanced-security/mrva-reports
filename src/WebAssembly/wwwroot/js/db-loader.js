/**
 * Fetches a gzip-compressed file and decompresses it using the browser's
 * native DecompressionStream API (runs in C++, much faster than .NET's
 * GZipStream compiled to WASM).
 *
 * Progress is reported to the #loading-bar and #loading-status elements
 * in index.html if they exist.
 *
 * @param {string} url - Relative or absolute URL of the .gz file.
 * @returns {Promise<Uint8Array>} The decompressed bytes.
 */
export async function fetchAndDecompress(url) {
    const bar = document.getElementById('loading-bar');
    const status = document.getElementById('loading-status');

    const setProgress = (pct, text) => {
        if (bar) bar.style.width = `${pct}%`;
        if (status) status.textContent = text;
    };

    // ── Download phase ──────────────────────────────────────────────
    setProgress(0, 'Downloading database…');

    const response = await fetch(url);
    if (!response.ok) {
        throw new Error(`Failed to fetch ${url}: ${response.status}`);
    }

    const contentLength = parseInt(response.headers.get('Content-Length') || '0', 10);
    const downloadReader = response.body.getReader();
    const compressedChunks = [];
    let downloadedBytes = 0;

    while (true) {
        const { done, value } = await downloadReader.read();
        if (done) break;
        compressedChunks.push(value);
        downloadedBytes += value.length;

        if (contentLength > 0) {
            const pct = Math.min(70, Math.round((downloadedBytes / contentLength) * 70));
            const mb = (downloadedBytes / 1048576).toFixed(1);
            const totalMb = (contentLength / 1048576).toFixed(0);
            setProgress(pct, `Downloading… ${mb} / ${totalMb} MB`);
        } else {
            const mb = (downloadedBytes / 1048576).toFixed(1);
            setProgress(35, `Downloading… ${mb} MB`);
        }
    }

    // ── Decompress phase ────────────────────────────────────────────
    setProgress(70, 'Decompressing…');

    // Reassemble the downloaded chunks into a single stream for
    // DecompressionStream to consume.
    const compressedBlob = new Blob(compressedChunks);
    const ds = new DecompressionStream('gzip');
    const decompressedStream = compressedBlob.stream().pipeThrough(ds);

    const reader = decompressedStream.getReader();
    const chunks = [];
    let totalLength = 0;

    while (true) {
        const { done, value } = await reader.read();
        if (done) break;
        chunks.push(value);
        totalLength += value.length;
    }

    setProgress(90, 'Preparing database…');

    const result = new Uint8Array(totalLength);
    let offset = 0;
    for (const chunk of chunks) {
        result.set(chunk, offset);
        offset += chunk.length;
    }

    setProgress(100, 'Ready');
    return result;
}
