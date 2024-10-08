<script>
    function downloadFile(streamRef, fileName) {
        const url = URL.createObjectURL(streamRef);
        const a = document.createElement('a');
        a.style.display = 'none';
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        URL.revokeObjectURL(url);
    }
</script>