var link = document.querySelector("link[rel*='icon']") || document.createElement("link");
link.type = "image/x-icon";
link.rel = "shortcut icon";
link.href = "/favicon.ico";
document.getElementsByTagName("head")[0].appendChild(link);

document.addEventListener('DOMContentLoaded', function () {
  setTimeout(function () {
    const linkElement = document.querySelector('div.topbar-wrapper > a[rel="noopener noreferrer"].link');
    if (linkElement) {
      const svgElement = linkElement.querySelector('svg');
      if (svgElement) {
        linkElement.parentNode.insertBefore(svgElement, linkElement);
      }
      linkElement.remove();
    }
  }, 100);
}, false);