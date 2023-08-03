mergeInto(LibraryManager.library, {

  SendMessageJS: function (str) {
    speak(UTF8ToString(str));
  },

  SendAlertJS: function (str) {
    window.alert(UTF8ToString(str));
  },

  ChangeButtonLabelJS: function (buttonId, buttonLabel) {
    changeButtonLabel(UTF8ToString(buttonId), UTF8ToString(buttonLabel));
  },

});