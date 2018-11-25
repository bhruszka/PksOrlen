<template>
  <div>
    <div id="map"></div>
    <div class="menu" style="z-index: 9; position: absolute; top: 8px;">
      <a href="/" class="button"><strong>Main Page</strong></a>
      <p style="background-color: white; padding: 5px; radius: 8px; margin-top: 8px;">
        Instrukcja:<br>
        Podwojne klikniecie na drogę - dodanie przystanku <br>
        Ponowne podwojne klikniecie na drogę - usunięcie przystanku <br>
      </p>
    </div>
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
        zoom: 14
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

      let result = await this.$http.get(`https://pksorlen.pl/api/edges/`);
      //   result.data.forEach(n => {
      //     this.addMarker(
      //       { lat: Number(n.latitude), lng: Number(n.longitude) },
      //       n.adjacent_nodes,
      //       n.id
      //     );
      //   });

      result.data.forEach(n => {
        this.createRoute(n.node_1, n.node_2, n.id, n.has_bus_stop);
      });
    },
    addMarker: function(latlng) {
      console.log(latlng);
      var marker = new google.maps.Marker({
        position: latlng,
        map: this.map,
        icon: "https://castdeo.ams3.cdn.digitaloceanspaces.com/intersection.png"
      });
    },
    addBusMarker: function(route) {
      if (route.has_bus_stop) {
        let path = route.getPath().getArray();
        let lat = (path[0].lat() + path[1].lat()) / 2;
        let lng = (path[0].lng() + path[1].lng()) / 2;

        let marker = new google.maps.Marker({
          position: { lat: lat, lng: lng },
          map: this.map
        });

        marker.route = route;
        self = this;
        google.maps.event.addListener(marker, "dblclick", function(event) {
          self.removeStop(this.route);
        });

        route.marker = marker;
      }
    },
    addStop: function(route) {
      if (route.has_bus_stop) {
        this.removeStop(route);
      } else {
        route.has_bus_stop = true;
        this.patchStop(route);
        this.addBusMarker(route);
      }
    },
    removeStop: function(route) {
      route.marker.setMap(null);
      route.has_bus_stop = false;
      this.patchStop(route);
    },
    patchStop: function(route) {
      this.$http
        .patch(`https://pksorlen.pl/api/edges/${route.id}/`, {
          has_bus_stop: route.has_bus_stop
        })
        .catch(error => {
          console.log(error);
          //   window.location.reload(false);
        });
    },
    createRoute: function(n1, n2, id, has_bus_stop = false) {
      var line = new google.maps.Polyline({
        path: [
          { lat: Number(n1.latitude), lng: Number(n1.longitude) },
          { lat: Number(n2.latitude), lng: Number(n2.longitude) }
        ],
        strokeColor: "#FF0000",
        strokeOpacity: 1.0,
        strokeWeight: 5,
        map: this.map
      });

      console.log(`id: ${id}`);
      line.id = id;
      line.has_bus_stop = has_bus_stop;

      this.addMarker({ lat: Number(n1.latitude), lng: Number(n1.longitude) });
      this.addMarker({ lat: Number(n2.latitude), lng: Number(n2.longitude) });

      if (line.has_bus_stop) {
        this.addBusMarker(line);
      }

      let self = this;
      google.maps.event.addListener(line, "dblclick", function(event) {
        self.addStop(this);
      });
    }
  }
};
</script>