<template>
  <div>
    <div id="map"></div>
    <div class="menu" style="z-index: 9; position: absolute; top: 8px;">
      <a href="/" class="button"><strong>Main Page</strong></a>
      <a @click="postTopo" class="button"><strong>Submit</strong></a>
      <p style="background-color: white; padding: 5px; radius: 8px; margin-top: 8px;">
        Instrukcja:<br>
        Podwojne klikniecie - dodawanie skrzyżowan <br>
        Click and drag - przesuwanie skrzyżowan <br>
        Pojedyncze klikniecie na skrzyzowanie i kolejne pojedyncz klikniecie na inne skrzyzowanie - dodanie drogi.
        Click submit - by zatwierdzić zmiany.
      </p>
    </div>
  </div>
</template>
<script>
import { createRoute, removeRoute, removeNode } from "../utils/utils.js";
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

      google.maps.event.addListener(this.map, "dblclick", function(event) {
        console.log(event);
        self.addMarker(event.latLng);
        event.stop();
      });

      google.maps.event.addListener(this.map, "click", function(event) {
        if (self.selectedNode != null) {
          self.selectedNode.setIcon(
            "https://castdeo.ams3.cdn.digitaloceanspaces.com/intersection.png"
          );
          self.selectedNode = null;
        }
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
        draggable: true,
        title: "Drag me!",
        icon: "https://castdeo.ams3.cdn.digitaloceanspaces.com/intersection.png"
      });
      let self = this;

      google.maps.event.addListener(marker, "click", function(event) {
        console.log(self.selectedNode);
        if (self.selectedNode == this) {
          self.selectedNode.setIcon(
            "https://castdeo.ams3.cdn.digitaloceanspaces.com/intersection.png"
          );
          self.selectedNode = null;
        } else {
          if (self.selectedNode != null) {
            self.addRoute(this, self.selectedNode);
          } else {
            self.selectedNode = this;
            this.setIcon(
              "https://castdeo.ams3.cdn.digitaloceanspaces.com/intersection_selected.png"
            );
          }
        }

        event.stop();
      });

      google.maps.event.addListener(marker, "dblclick", function(event) {
        console.log(self.nodes.indexOf(marker));
        self.nodes.splice(self.nodes.indexOf(marker), 1);
        if (self.selectedNode != null) {
          self.selectedNode.setIcon(
            "https://castdeo.ams3.cdn.digitaloceanspaces.com/intersection.png"
          );
        }
        self.selectedNode = null;
        removeNode(this);
        event.stop();
      });

      google.maps.event.addListener(marker, "dragend", function() {
        let routes = this.routes.slice();
        routes.forEach(r => {
          let line = r.line;
          removeRoute(r.line);
          createRoute(line.n1, line.n2, this.map);
        });
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
    addRoute: function(n1, n2) {
      let index = n1.routes.findIndex(x => x.line.n1 == n2 || x.line.n2 == n2);
      console.log(index);
      if (index != -1) {
        removeRoute(n1.routes[index].line);
      } else {
        createRoute(n1, n2, this.map);
      }
    },
    addRouteNoRemove: function(n1, n2) {
      let index = n1.routes.findIndex(x => x.line.n1 == n2 || x.line.n2 == n2);
      if (index != -1) {
        return;
      } else {
        createRoute(n1, n2, this.map);
      }
    },
    postTopo: async function() {
      let data = [];
      let data_patch = [];
      this.nodes.forEach((n, index) => {
        n.id = index + 1;
      });
      this.nodes.forEach((n, index) => {
        let adjacent_nodes = n.routes.map(
          x => x.line[x.index == 1 ? "n1" : "n2"].id
        );
        data.push({
          id: n.id,
          longitude: n.position.lng(),
          latitude: n.position.lat()
          //   adjacent_nodes: adjacent_nodes
        });
        data_patch.push({
          id: n.id,
          adjacent_nodes: adjacent_nodes
        });
      });
      await this.$http.post("https://pksorlen.pl/api/nodes/", data);
      await this.$http.patch(
        "https://pksorlen.pl/api/nodes-bulk-patch/",
        data_patch
      );
    }
  }
};
</script>
<style>
/* Always set the map height explicitly to define the size of the div
       * element that contains the map. */
#map {
  height: 100vh;
}
/* Optional: Makes the sample page fill the window. */
html,
body {
  height: 100%;
  margin: 0;
  padding: 0;
}
</style>