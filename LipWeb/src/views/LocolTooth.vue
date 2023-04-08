<template>
  <div>
    <v-btn style="margin: 5px" @click="refresh">刷新 </v-btn>
    <span style="vertical-align: middle"> 当前：{{ currentPathName }} </span>
    <div v-if="toothlist">
      <v-scroll-x-transition
        class="tooth-item-card"
        v-for="pkg in toothlist.packages"
      >
        <v-card
          :title="pkg.information.name + '@' + pkg.version"
          :subtitle="pkg.information.description"
        >
          <div style="margin: 20px; margin-top: 0px">
            <p>作者：{{ pkg.information.author }}</p>
            <p v-if="pkg.information.homepage">
              主页：{{ pkg.information.homepage }}
            </p>
            <p v-if="pkg.information.license">
              协议：{{ pkg.information.license }}
            </p>
          </div>
        </v-card>
      </v-scroll-x-transition>
    </div>
    <div v-else>
      <v-overlay>
        <v-progress-circular indeterminate color="primary" />
      </v-overlay>
    </div>
  </div>
</template>
<script lang="ts" setup>
import api from "@/api";
import { ToothItemResult } from "@/api/models";
import { useGlobal } from "@/store";
import { computed, ref } from "vue";
const data = useGlobal();
const toothlist = ref<ToothItemResult | null>();
const currentPathName = computed(() => {
  if (data.token) {
    api
      .getToothList()
      .then((result) => {
        toothlist.value = result;
      })
      .catch((e) => {
        data.message = e;
      });
  }
  return data.allPath.find((v) => v.value === data.currentPath)?.name ?? "";
});
const refresh = () => {
  toothlist.value = null;
  api
    .getToothList()
    .then((result) => {
      toothlist.value = result;
    })
    .catch((e) => {
      data.message = e;
    });
};
</script>
<style scoped>
.tooth-item-card {
  margin: 10px;
}
</style>
