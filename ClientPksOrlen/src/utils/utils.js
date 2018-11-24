export function removeRoute(line) {
  line.n1.routes.splice(
    line.n1.routes.findIndex(x => x.line == line),
    1
  );
  line.n2.routes.splice(
    line.n2.routes.findIndex(x => x.line == line),
    1
  );
  line.setMap(null);
}

export function createRoute(n1, n2, map) {
  var line = new google.maps.Polyline({
    path: [n1.position, n2.position],
    strokeColor: "#FF0000",
    strokeOpacity: 1.0,
    strokeWeight: 5,
    map: map
  });

  line.n1 = n1;
  line.n2 = n2;
  n1.routes.push({ line: line, index: 0 });
  n2.routes.push({ line: line, index: 1 });

  google.maps.event.addListener(line, "dblclick", function(event) {
    removeRoute(this);
  });
}

export function removeNode(node) {
    let routes = node.routes.slice();
    routes.forEach(r => {
        removeRoute(r.line);
    });
    node.setMap(null);
}