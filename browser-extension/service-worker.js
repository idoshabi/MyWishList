chrome.action.onClicked.addListener(async (tab) => {
  if (!tab.id || !tab.url) {
    return;
  }

  await chrome.scripting.executeScript({
    target: { tabId: tab.id },
    func: () => {
      const productName =
        document.querySelector("meta[property='og:title']")?.content ||
        document.title ||
        "Imported Item";
      const currentUrl = window.location.href;
      const destination = `https://mywishlist-68210.azurewebsites.net/discover?importName=${encodeURIComponent(productName)}&importUrl=${encodeURIComponent(currentUrl)}`;
      window.open(destination, "_blank");
    },
  });
});
