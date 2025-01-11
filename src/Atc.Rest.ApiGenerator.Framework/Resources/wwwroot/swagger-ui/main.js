let link = document.querySelector("link[rel*='icon']") || document.createElement("link");
link.type = "image/x-icon";
link.rel = "shortcut icon";
link.href = "/favicon.ico";
document.getElementsByTagName("head")[0].appendChild(link);

document.addEventListener('DOMContentLoaded', function () {
  setTimeout(function () {
    const svgElement = document.querySelector('div.topbar-wrapper > a[rel="noopener noreferrer"].link > svg');
    if (svgElement) {
      svgElement.remove()
    }
  }, 100);
}, false);