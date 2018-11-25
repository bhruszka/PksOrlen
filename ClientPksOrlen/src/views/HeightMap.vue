<template>
    <div>
        <div id="map"></div>
        <div class="menu" style="z-index: 9; position: absolute; top: 8px;">
            <a href="/" class="button"><strong>Main Page</strong></a>
            <a class="button" href="/width">Mapa szerokości dróg</a>
            <a class="button" href="/weight">Mapa dopuszczalnej wagi pojazdow.</a>
            <p style="background-color: white; padding: 5px; radius: 8px; margin-top: 8px;">
                Mapa dopuszczalnej wysokosci pojazdow.
                Szary - brak danych.
            </p>
        </div>
    </div>
</template>
<script>
export default {
  data() {
    return {
      map: null,
      start: null,
      finish: null,
      my_data: null,
      redirect_id: null
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
        this.createRoute(
          n.node_1,
          n.node_2,
          n.id,
          n.max_height,
          n.has_bus_stop
        );
      });
    },
    addMarker: function(latlng, id) {
      var marker = new google.maps.Marker({
        position: latlng,
        map: this.map,
        icon: "https://castdeo.ams3.cdn.digitaloceanspaces.com/intersection.png"
      });
      marker.id = id;
      marker.type = "node";
      let self = this;
    },
    createRoute: function(n1, n2, id, width, has_bus_stop = false) {
      console.log(width);
      var line = new google.maps.Polyline({
        path: [
          { lat: Number(n1.latitude), lng: Number(n1.longitude) },
          { lat: Number(n2.latitude), lng: Number(n2.longitude) }
        ],
        strokeColor: width == null ? "#AAAAAA" : "#FF0000",
        strokeOpacity: 1.0,
        title: `Width: ${width == null ? 2.0 : width}`,
        strokeWeight: width == null ? 2.0 : width,
        map: this.map
      });

      line.id = id;
      line.has_bus_stop = has_bus_stop;
      line.type = "edge";

      this.addMarker(
        { lat: Number(n1.latitude), lng: Number(n1.longitude) },
        n1.id
      );
      this.addMarker(
        { lat: Number(n2.latitude), lng: Number(n2.longitude) },
        n2.id
      );
    }
  }
};
</script>
<style>
.menu {
  z-index: 9;
  position: absolute;
  top: 8px;
  left: 50%;
  transform: translate(-50%, 0);
}
</style>
