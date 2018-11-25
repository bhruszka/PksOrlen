<template>
  <div>
    <div id="map"></div>
    <div class="menu" style="z-index: 9; position: absolute; top: 8px;">
      <a v-if="redirect_id != null" class="button" href=""><strong>Wypłenij szczegóły trasy ciężarówki</strong></a>

      <a @click="postTruck" class="button"><strong>Submit</strong></a>
      <a @click="reset" class="button"><strong>Reset</strong></a>
      <p style="background-color: white; padding: 5px; radius: 8px; margin-top: 8px;">
        Instrukcja:<br>
        Wybierz poczatek i koniec trasy - obie wartosci moga byc droga lub skrzyzowaniem.
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
    addMarker: function(latlng, id) {
      console.log(latlng);
      var marker = new google.maps.Marker({
        position: latlng,
        map: this.map,
        icon: "https://castdeo.ams3.cdn.digitaloceanspaces.com/intersection.png"
      });
      marker.id = id;
      marker.type = "node";
      let self = this;
      google.maps.event.addListener(marker, "click", function(event) {
        console.log("node");
        if (self.start == null) {
          self.start = this;
          this.setIcon(
            "https://castdeo.ams3.cdn.digitaloceanspaces.com/intersection_selected.png"
          );
        } else if (self.finish == null) {
          self.finish = this;
          this.setIcon(
            "https://castdeo.ams3.cdn.digitaloceanspaces.com/intersection_selected.png"
          );
        }
        event.stop();
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
      line.type = "edge";

      this.addMarker(
        { lat: Number(n1.latitude), lng: Number(n1.longitude) },
        n1.id
      );
      this.addMarker(
        { lat: Number(n2.latitude), lng: Number(n2.longitude) },
        n2.id
      );

      let self = this;
      google.maps.event.addListener(line, "click", function(event) {
        if (self.start == null) {
          self.start = this;
          this.setOptions({ strokeColor: "blue" });
        } else if (self.finish == null) {
          self.finish = this;
          this.setOptions({ strokeColor: "blue" });
        }
        event.stop();
      });
    },
    postTruck: async function() {
      if (this.start == null || this.finish == null) {
        return;
      }

      let result = await this.$http.get(
        `https://pksorlen.pl/api/generate-route/?start_id=${
          this.start.id
        }&start_type=${this.start.type}&finish_id=${
          this.finish.id
        }&finish_type=${this.finish.type}`
      );
      console.log(result.data.id);
      self.redirect_id = result.data.id;
      window.open(
        `https://pksorlen.pl/admin/router/node/${result.data.id}/change/`,
        "_self"
      );
    },
    reset: function() {
      console.log("reset");
      console.log(this.start, this.finish);
      console.log(this.start.type, this.finish.type);
      if (this.start != null && this.start.type == "node") {
        this.start.setIcon(
          "https://castdeo.ams3.cdn.digitaloceanspaces.com/intersection.png"
        );
      }

      if (this.finish != null && this.finish.type == "node") {
        this.finish.setIcon(
          "https://castdeo.ams3.cdn.digitaloceanspaces.com/intersection.png"
        );
      }

      if (this.start != null && this.start.type == "edge") {
        this.start.setOptions({ strokeColor: "red" });
      }

      if (this.finish != null && this.finish.type == "edge") {
        this.finish.setOptions({ strokeColor: "red" });
      }

      this.start = null;
      this.finish = null;
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
