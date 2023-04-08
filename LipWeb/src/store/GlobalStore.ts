import { computed, ComputedRef, ref, type Ref } from "vue";
import { defineStore } from "pinia";

/** Global Store */
const useGlobalStore = defineStore("global", () => {
  return {
    currentPath: ref("path2"),
    allPath: ref<{ name: string; value: string }[]>([
      { name: "目录1", value: "path1" },
      { name: "目录2", value: "path2" },
      { name: "目录3", value: "path3" },
    ]),
    loading: ref(true),
    progress: ref<number | null>(null),
    message: ref(""),
    token: ref(""),
    apiPath: ref(""),
  };
});
export default useGlobalStore;
