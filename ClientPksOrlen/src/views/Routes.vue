<template>
    <div>
        <div id="map"></div>
        <button style="z-index: 9; position: absolute; top: 50px;">Submit</button>
    </div>
</template>
<script>
export default {
  data() {
    return {
      map: null,
      nodes: [],
      selectedNode: null
    };
  },
  mounted() {
    let script = document.createElement("script");
    script.onload = () => {};
    script.async = true;
    script.src =
      "https://maps.googleapis.com/maps/api/js?key=AIzaSyBbU8a4ySNmGepBLM1YJXMCZr8lMDu6GRU&callback=initMap";
    document.head.appendChild(script);

    window.initMap = this.initMap;
  },
  methods: {
    initMap: async function() {
      this.map = new google.maps.Map(document.getElementById("map"), {
        center: { lat: 52.588262, lng: 19.67104 },
        zoom: 15
      });

      let opt = {
        minZoom: 13,
        disableDoubleClickZoom: true
      };
      this.map.setOptions(opt);
      let strictBounds = new google.maps.LatLngBounds(
        new google.maps.LatLng(52.570627, 19.646783),
        new google.maps.LatLng(52.607317, 19.708986)
      );

      let self = this;
      google.maps.event.addListener(this.map, "dragend", function() {
        if (strictBounds.contains(self.map.getCenter())) return;

        // We're out of bounds - Move the map back within the bounds
        console.log(self.map);
        var c = self.map.getCenter(),
          x = c.lng(),
          y = c.lat(),
          maxX = strictBounds.getNorthEast().lng(),
          maxY = strictBounds.getNorthEast().lat(),
          minX = strictBounds.getSouthWest().lng(),
          minY = strictBounds.getSouthWest().lat();

        if (x < minX) x = minX;
        if (x > maxX) x = maxX;
        if (y < minY) y = minY;
        if (y > maxY) y = maxY;

        self.map.setCenter(new google.maps.LatLng(y, x));
      });

      let result = await this.$http.get(`https://pksorlen.pl/api/nodes/`);
      result.data.forEach(n => {
        this.addMarker(
          { lat: Number(n.latitude), lng: Number(n.longitude) },
          n.adjacent_nodes,
          n.id
        );
      });

      this.nodes.forEach(n => {
        n.adjacent_nodes.forEach(a => {
          this.addRouteNoRemove(
            n,
            this.nodes.find(m => {
              if (m.id == a) return m;
            })
          );
        });
      });
    },
    addMarker: function(latlng, adjacent_nodes = null, id = null) {
      console.log(latlng);
      var marker = new google.maps.Marker({
        position: latlng,
        map: this.map,
        icon: "https://castdeo.ams3.cdn.digitaloceanspaces.com/intersection.png"
      });

      marker.routes = [];
      if (adjacent_nodes != null) {
        marker.adjacent_nodes = adjacent_nodes;
      }
      if (id != null) {
        marker.id = id;
      }

      this.nodes.push(marker);
    },
    addStop: function(route) {
      if (route.stop) {
        route.marker.setMap(null);
        route.stop = false;
      } else {
        let path = route.getPath().getArray();
        let lat = (path[0].lat() + path[1].lat()) / 2;
        let lng = (path[0].lng() + path[1].lng()) / 2;

        let marker = new google.maps.Marker({
          position: { lat: lat, lng: lng },
          map: this.map
        });

        marker.route = route;

        google.maps.event.addListener(marker, "dblclick", function(event) {
          this.route.marker.setMap(null);
          this.route.stop = false;
        });

        route.stop = true;
        route.marker = marker;
      }
    },
    addRouteNoRemove: function(n1, n2) {
      let index = n1.routes.findIndex(x => x.line.n1 == n2 || x.line.n2 == n2);
      if (index != -1) {
        return;
      } else {
        this.createRoute(n1, n2);
      }
    },
    createRoute: function(n1, n2) {
      var line = new google.maps.Polyline({
        path: [n1.position, n2.position],
        strokeColor: "#FF0000",
        strokeOpacity: 1.0,
        strokeWeight: 5,
        map: this.map
      });

      line.n1 = n1;
      line.n2 = n2;
      n1.routes.push({ line: line, index: 0 });
      n2.routes.push({ line: line, index: 1 });

      let self = this;
      google.maps.event.addListener(line, "dblclick", function(event) {
        self.addStop(this);
      });
    },
    postStops: async function() {
      let data = [];
      this.nodes.forEach((n, index) => {
        n.id = index + 1;
      });
      this.nodes.forEach((n, index) => {
        let adjacent_nodes = n.routes.map(x => x.id);
        data.push({
          id: n.id,
          longitude: n.position.lng(),
          latitude: n.position.lat()
        });
      });
      await this.$http.post("https://pksorlen.pl/api/nodes/", data);

      this.nodes.forEach((n, index) => {
        let adjacent_nodes = n.routes.map(
          x => x.line[x.index == 1 ? "n1" : "n2"].id
        );
        this.$http.patch(`https://pksorlen.pl/api/nodes/${n.id}/`, {
          pk: n.id,
          adjacent_nodes: adjacent_nodes
        });
      });
    }
  }
};
</script>