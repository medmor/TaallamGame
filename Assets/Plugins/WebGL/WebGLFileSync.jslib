mergeInto(LibraryManager.library, {
  SyncFiles: function() {
    FS.syncfs(false, function (err) {
      if (err) console.error('IDBFS sync (flush) error:', err);
    });
  },
  LoadFiles: function() {
    FS.syncfs(true, function (err) {
      if (err) console.error('IDBFS sync (load) error:', err);
    });
  }
});
