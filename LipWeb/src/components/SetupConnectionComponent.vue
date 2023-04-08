<template>
  <v-hover v-slot="{ isHovering, props }" open-delay="200">
    <v-card
      :elevation="isHovering ? 16 : 2"
      :class="{ 'on-hover': isHovering }"
      :style="{ padding: '15px' }"
      class="mx-auto"
      v-bind="props"
    >
      <v-sheet width="300" class="mx-auto">
        <v-form ref="form" @submit.prevent>
          <v-text-field
            v-model="url"
            clearable
            :rules="[checkValid]"
            label="后端地址"
          ></v-text-field>
          <v-expand-transition>
            <v-progress-linear
              style="margin-top: -24px"
              v-if="connecting"
              color="deep-purple-accent-4"
              indeterminate
              rounded
              height="6"
            ></v-progress-linear>
          </v-expand-transition>
          <v-expand-transition>
            <div style="text-align: center" v-if="message">
              {{ message }}
            </div>
          </v-expand-transition>
        </v-form>
      </v-sheet>
    </v-card>
  </v-hover>
</template>
<script lang="ts">
import api from "@/api";
import useGlobalStore from "@/store/GlobalStore";
let currentUrl = window.location.protocol + "//" + window.location.host;
export default {
  data: (): {
    message: string | undefined;
    connecting: boolean;
    url: string;
  } => ({
    message: "填写后端地址后会自动完成连接",
    connecting: false,
    url: currentUrl,
  }),
  methods: {
    async tryConnect(value: string) {
      (this.$refs as any).form.resetValidation();
      const result = await (async () => {
        try {
          if (!value) return "请填写后端地址";
          if (!value.startsWith("http")) return "请填写一个完整的后端http地址";
          api.base.setAxiosConfig(value);
          if (await api.base.verifyBackend()) {
            this.message = "连接成功";
            useGlobalStore().apiPath = value;
          } else return "后端地址无法访问";
          return true;
        } catch (error) {
          return "错误：" + error;
        }
      })();
      return result;
    },
    async checkValid(value: string) {
      try {
        this.connecting = true;
        this.message = "正在连接";
        return await this.tryConnect(value);
      } finally {
        this.message = undefined;
        this.connecting = false;
      }
    },
  },
  mounted() {
    this.tryConnect(currentUrl).catch((e) => {
      console.error(e);
    });
  },
};
</script>
