<template>
  <v-card class="mx-auto px-6 py-8" min-width="300">
    <v-form v-model="form" @submit.prevent="onSubmit">
      <v-text-field
        id="username-tb"
        v-model="username"
        :readonly="loading"
        :rules="[required]"
        class="mb-2"
        clearable
        label="用户名"
      ></v-text-field>
      <v-text-field
        v-model="password"
        :readonly="loading"
        :rules="[required]"
        clearable
        label="密钥"
        placeholder="输入密码"
      ></v-text-field>
      <v-btn
        :disabled="!form"
        :loading="loading"
        block
        color="success"
        size="large"
        type="submit"
        variant="elevated"
      >
        登录
      </v-btn>
    </v-form>
  </v-card>
</template>
<script lang="ts">
import api from "@/api";
import { useGlobal } from "@/store";
export default {
  data: () => ({
    form: false,
    username: "",
    password: "",
    loading: false,
  }),
  methods: {
    async onSubmit() {
      if (!this.form) return;
      this.loading = true;
      const result = await api.base.auth(this.username, this.password);
      if (result.success) {
        useGlobal().token = result.token;
        //  useGlobal().message = "登录成功";
      } else {
        useGlobal().message = result.message;
      }
      this.loading = false;
    },
    required(v: string) {
      return !!v || "请输入";
    },
  },
  mounted() {
    this.$nextTick(() => {
      document.getElementById("username-tb")?.focus();
    });
  },
};
</script>
