<template>
  <v-card class="mx-auto px-6 py-8" min-width="300">
    <v-form v-model="form" @submit.prevent="onSubmit">
      <v-text-field
        v-model="email"
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

      <br />

      <v-btn
        :disabled="!form"
        :loading="loading"
        block
        color="success"
        size="large"
        type="submit"
        variant="elevated"
      >
        Sign In
      </v-btn>
    </v-form>
  </v-card>
</template>
<script lang="ts">
export default {
  data: () => ({
    form: false,
    email: null,
    password: null,
    loading: false,
  }),

  methods: {
    onSubmit() {
      if (!this.form) return;

      this.loading = true;

      setTimeout(() => (this.loading = false), 2000);
    },
    required(v: string) {
      return !!v || "Field is required";
    },
  },
};
</script>
