import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import axios from "axios";
import "./registerServiceWorker";

Vue.config.productionTip = false;
axios.defaults.headers.common['Authorization'] = 'Token ' + localStorage.getItem('token');
Vue.prototype.$http = axios;

new Vue({
  router,
  render: h => h(App)
}).$mount("#app");
