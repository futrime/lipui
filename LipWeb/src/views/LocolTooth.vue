<template>
  <div>
    <div v-if="!toothlist" class="progress-circular">
      <v-progress-linear size="64" indeterminate color="blue-accent-3" />
    </div>
    <div style="margin-left: 20px; margin-top: 20px">
      <v-btn variant="text" @click="refresh">点击刷新 </v-btn>
      <span style="vertical-align: middle"> 当前：{{ currentPathName }} </span>
    </div>
    <v-fade-transition v-if="toothlist" v-for="pkg in toothlist.packages">
      <div class="tooth-item-card">
        <tooth-item v-bind:value="pkg" v-bind:actions="buttons" />
      </div>
    </v-fade-transition>
  </div>
</template>
<script lang="ts" setup>
import api from "@/api";
import ToothItem from "@/components/ToothItem.vue";
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
const buttons = [
  {
    color: "primary",
    text: "检查更新",
    callback: () => {
      console.log("检查更新");
    },
  },
  {
    color: "secondary",
    text: "卸载",
    callback: () => {
      console.log("卸载");
    },
  },
];
</script>
<style scoped>
.tooth-item-card {
  margin: 10px;
}

.progress-circular {
  position: fixed;
}
</style>
