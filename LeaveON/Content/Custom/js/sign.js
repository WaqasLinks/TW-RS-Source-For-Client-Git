paintCanvas = document.getElementById("digSignCanvas");
paintContext = paintCanvas.getContext("2d");

activeColour = "black";
dragging = false;
cursorPoint = { x: 0, y: 0 };

paintCanvas.onmousedown = (e) => { onMouseDownHandler(e); };
paintCanvas.onmouseup = (e) => { onMouseUpHandler(e); };
paintCanvas.onmouseout = (e) => { onMouseUpHandler(e); };
paintCanvas.onmousemove = (e) => { onMouseMoveHandler(e); };

const canvas = paintCanvas;

document.body.addEventListener("touchstart", (e) => {

  if (e.target == canvas) {
    document.getElementById("digSigLab").innerHTML = "BONJOUR";
    e.preventDefault();
    onMouseDownHandler(e);
  }
}, false);

document.body.addEventListener("touchend", (e) => {
  if (e.target == canvas) {
    e.preventDefault();
    onMouseUpHandler(e);
  }
}, false);

document.body.addEventListener("touchmove", (e) => {
  if (e.target == canvas) {
    e.preventDefault();
    onMouseMoveHandler(e);
  }
}, false);

function clear() {
  paintContext.clearRect(0, 0, 100000, 100000);
}

document.getElementById("clearSign").addEventListener("click", clear);

function getLocationFrom(e) {
  const location = { x: 0, y: 0 };
  console.log(e.constructor.name);

  if (e.constructor.name == "MouseEvent") {
    location.x = e.offsetX;
    location.y = e.offsetY;
  } else {
    const bounds = e.target.getBoundingClientRect();
    const touch = e.targetTouches[0];

    location.x = touch.pageX - bounds.left;
    location.y = touch.pageY - bounds.top;
  }

  return location;
}

function onMouseDownHandler(e) {
  dragging = true;

  const location = getLocationFrom(e);
  cursorPoint.x = location.x;
  cursorPoint.y = location.y;

  paintContext.lineWidth = 1;
  paintContext.lineCap = 'round';
  paintContext.filter = 'blur(1px)';
  paintContext.beginPath();
  paintContext.moveTo(cursorPoint.x, cursorPoint.y);
  paintContext.strokeStyle = activeColour;
}

function onMouseUpHandler() {
  dragging = false;
}

function onMouseMoveHandler(e) {
  if (!dragging) return;

  const location = getLocationFrom(e);
  paintContext.lineTo(location.x, location.y);
  paintContext.stroke();
}

function toString() {
  return paintCanvas.toDataURL("image/png");
}
