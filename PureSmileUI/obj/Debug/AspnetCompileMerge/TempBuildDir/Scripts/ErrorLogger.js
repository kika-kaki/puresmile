var ErrorLogger =
{
  LogInfo: function (message) {
    ErrorLogger.LogInternal(message);
  },

  LogError: function (message, file, line) {
    ErrorLogger.LogInternal(JSON.stringify(message), file, line);
  },
  
  LogInternal: function(msg, file, line) {
    $.ajax({
      method: "POST",
      url: "/JsLog/Log",
      data: { message: msg, file: file, line: line }
    });
  }
}