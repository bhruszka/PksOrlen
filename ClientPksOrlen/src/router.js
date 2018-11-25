import Vue from "vue";
import Router from "vue-router";
import Home from "./views/Home.vue";
import Map from "./components/Map.vue";
import Stops from "./views/Stops.vue";
import Routes from "./views/Routes.vue";
import Trucks from "./views/Trucks.vue";

Vue.use(Router);

export default new Router({
  mode: "history",
  base: process.env.BASE_URL,
  routes: [
    {
      path: "/",
      name: "home",
      component: Home
    },
    {
      path: "/map",
      name: "map",
      component: Map
    },
    {
      path: "/stops",
      name: "stops",
      component: Stops
    },
    {
      path: "/routes",
      name: "routes",
      component: Routes
    },
    {
      path: "/trucks",
      name: "trucks",
      component: Trucks
    },
    {
      path: "/about",
      name: "about",
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () =>
        import(/* webpackChunkName: "about" */ "./views/About.vue")
    }
  ]
});
