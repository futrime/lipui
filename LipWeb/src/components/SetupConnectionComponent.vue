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
            v-model="firstName"
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
export default {
  data: (): {
    message: string | undefined;
    connecting: boolean;
    firstName: string;
  } => ({
    message: "填写后端地址后会自动完成连接",
    connecting: false,
    firstName: "",
  }),
  methods: {
    async checkValid(value: string) {
      this.connecting = true;
      this.message = "正在连接";
      (this.$refs as any).form.resetValidation();
      const result = await (async () => {
        try {
          if (!value) return "请填写后端地址";
          if (!value.startsWith("http")) return "请填写一个完整的后端http地址";
          api.setAxiosConfig(value);
          if (await api.verifyBackend()) {
            this.message = "连接成功";
            useGlobalStore().apiPath = value;
          } else return "后端地址无法访问";
          return true;
        } catch (error) {
          return "错误：" + error;
        }
      })();
      this.message = undefined;
      this.connecting = false;
      return result;
    },
  },
};
</script>
