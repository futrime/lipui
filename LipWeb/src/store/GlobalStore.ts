import { computed, ComputedRef, ref, type Ref } from "vue";
import { defineStore } from "pinia";

/** Global Store */
const useGlobalStore = defineStore("global", () => {
  const currentPath: Ref<string> = ref("path2");
  const allPath: Ref<string[]> = ref(["path1", "path2", "path3"]);
  /** Loading overlay */
  const loading: Ref<boolean> = ref(true);
  /** ProgressBar Percentage */
  const progress: Ref<number | null> = ref(null);
  /** SnackBar Text */
  const message: Ref<string> = ref("");
  // Actions
  /**
   * Show loading Overlay
   *
   * @param display - visibility
   */
  function setLoading(display: boolean): void {
    loading.value = display;
    if (!display) {
      // Reset Progress value
      progress.value = null;
    }
  }
  /**
   * Update progress value
   *
   * @param v - progress value
   */
  function setProgress(v: number | null = null): void {
    // update progress value
    progress.value = v;
    // display loading overlay
    loading.value = true;
  }
  /**
   * Show snackbar message
   *
   * @param msg - snackbar message
   */
  function setMessage(msg: string = ""): void {
    // put snackbar text
    message.value = msg;
  }
  return {
    currentPath,
    allPath,
    loading,
    progress,
    message,
    token: ref(""),
    apiPath: ref(""),
    setLoading,
    setProgress,
    setMessage,
  };
});
export default useGlobalStore;
