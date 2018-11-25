<template>
  <div class="section">
    <p class="is-size-4">Menu: <br></p>
    <div class="container">
      <a class="button is-fullwidth my-button" href="/map">Zarzadzaj rozmieszczeniem dróg</a>
      <a class="button is-fullwidth my-button" href="/routes">Zarzadzaj właściwościami dróg</a>
      <a class="button is-fullwidth my-button" href="/stops">Zarzadzaj przystankami autobusowymi</a>
      <a class="button is-fullwidth my-button" href="/trucks">Dodaj przejazd ciężarówki</a>
      <a class="button is-fullwidth my-button" href="/width">Mapa szerokości dróg</a>
      <a class="button is-fullwidth my-button" href="/height">Mapa dopuszczalnej wysokosci pojazdow</a>
      <a class="button is-fullwidth my-button" href="/weight">Mapa dopuszczalnej wagi pojazdow.</a>
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
<style>
.my-button {
  padding: 20px !important;
  margin: 12px !important;
}
</style>

