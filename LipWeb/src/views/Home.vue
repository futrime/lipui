<template>
  <v-card subtitle="已连接到Lip" text="前往其他页面进行操作" variant="tonal" />
  <div class="main">
    选择工作目录：{{ data.currentPath }}
    <working-path-selector
      v-bind:source="data.allPath"
      v-model:value="currentPath"
    />
  </div>
</template>
<script lang="ts" setup>
import { useGlobal } from "@/store";
import WorkingPathSelector from "@/components/WorkingPathSelector.vue";
import { computed, watch } from "vue";
import api from "@/api";
const data = useGlobal();
const currentPath = computed({
  get: () => data.currentPath,
  set: (v: string) => {
    data.loading = true;
    api
      .setWorkingDirectory(v)
      .then((result) => {
        data.currentPath = result.value;
        data.allPath = result.directories;
      })
      .catch((e) => {
        data.message = e;
      })
      .finally(() => {
        data.loading = false;
      });
  },
});
watch(
  () => data.token,
  (v) => {
    if (v) {
      api
        .getWorkingDirectory()
        .then((result) => {
          data.currentPath = result.current ?? result.directories[0].value;
          data.allPath = result.directories;
        })
        .catch((e) => {
          data.message = e;
        });
    }
  }
);
</script>
<style lang="scss">
div.main {
  margin: 20px;
}
</style>
