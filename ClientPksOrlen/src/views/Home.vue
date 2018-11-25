<template>
  <div class="section">
    <div class="container">
      <a class="button is-fullwidth" href="/map">Zarzadzaj rozmieszczeniem dróg</a>
      <a class="button is-fullwidth" href="/routes">Zarzadzaj właściwościami dróg</a>
      <a class="button is-fullwidth" href="/stops">Zarzadzaj przystankami autobusowymi</a>
      <a class="button is-fullwidth" href="/trucks">Dodaj przejazd ciężarówki</a>
      <a class="button is-fullwidth" href="/width">Mapa szerokości dróg</a>
      <a class="button is-fullwidth" href="/height">Mapa dopuszczalnej wysokosci pojazdow</a>
      <a class="button is-fullwidth" href="/weight">Mapa dopuszczalnej wagi pojazdow.</a>
    </div>
  </div>
</template>

<script>
// @ is an alias to /src
import HelloWorld from "@/components/HelloWorld.vue";

export default {
  name: "home",
  components: {
    HelloWorld
  }
};
</script>
