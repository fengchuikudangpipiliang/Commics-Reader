<template>
  <div class="login-dialog-content">
    <div class="login-header">
      <h2>Welcome!<br />Sign in to your Account</h2>
      <el-button v-if="isLoggedIn" type="danger" size="small" circle @click="logout">
        <el-icon><i class="el-icon-switch-button"></i></el-icon>
      </el-button>
    </div>
    <el-form
      ref="formRef"
      :model="form"
      :rules="rules"
      status-icon
      label-width="0"
      class="my-el-form"
    >
      <el-form-item prop="username">
        <el-input v-model="form.username" placeholder="Account" />
      </el-form-item>
      <el-form-item prop="password">
        <el-input v-model="form.password" type="password" placeholder="Password" />
      </el-form-item>
      <div class="forgot-password-container">
        <a href="#" class="forgot-password">Forgot Password?</a>
      </div>
      <el-button type="primary" class="login-btn" @click="submitFunc">SIGN IN</el-button>
      <el-button type="info" class="signup-btn" @click="registerFunc">SIGN UP</el-button>
    </el-form>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import axios from 'axios'
import { useUserStore } from '@/store/login'

const emit = defineEmits(['close-dialog'])
const props = defineProps({
  isLoggedIn: {
    type: Boolean,
    default: false,
  },
})

const formRef = ref(null)
const form = ref({
  username: '',
  password: '',
})

const rules = {
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    {
      validator: async (rule, value, callback) => {
        if (!/^[A-Za-z0-9_]{3,20}$/.test(value)) {
          callback(new Error('用户名3-20位，仅限字母、数字、下划线'))
          return
        }
        callback()
      },
      trigger: 'blur',
    },
  ],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' },
    {
      validator: (rule, value, callback) => {
        if (!/^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$/.test(value)) {
          callback(new Error('密码需6位以上，并包含字母和数字'))
          return
        }
        callback()
      },
      trigger: 'blur',
    },
  ],
}

const userStore = useUserStore()

const submitFunc = () => {
  if (props.isLoggedIn) {
    ElMessage.info('您已登录')
    emit('close-dialog')
    return
  }

  formRef.value.validate((valid) => {
    if (valid) {
      axios
        .post('https://localhost:7274/api/Login/Login', {
          username: form.value.username,
          password: form.value.password,
        })
        .then((res) => {
          if (res.status === 200) {
            userStore.setUserInfo(res.data)
            ElMessage.success('登录成功')
            emit('close-dialog')
          } else {
            ElMessage.error(res.data?.msg || '登录失败')
          }
        })
        .catch((err) => {
          console.error('登录请求失败:', err)
          ElMessage.error('登录请求失败')
        })
    }
  })
}

const registerFunc = () => {
  formRef.value.validate(async (valid) => {
    if (valid) {
      try {
        let res = await axios.post('https://localhost:7274/api/Login/Rejister', {
          username: form.value.username,
          password: form.value.password,
        })
        if (res.status === 200) {
          ElMessage.success('注册成功')
        } else {
          ElMessage.error(res.data?.msg || '注册失败')
        }
      } catch (e) {
        console.error('注册请求失败:', e)
        ElMessage.error('注册请求失败')
      }
    }
  })
}

const logout = () => {
  ElMessageBox.confirm('您确定要退出登录吗？', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning',
  })
    .then(() => {
      userStore.clearUserInfo()
      ElMessage.success('您已退出登录')
      emit('close-dialog')
    })
    .catch(() => {
      ElMessage.info('已取消退出登录')
    })
}

watch(
  () => props.isLoggedIn,
  (newVal) => {
    if (!newVal) {
      form.value.username = ''
      form.value.password = ''
      if (formRef.value) {
        formRef.value.resetFields()
      }
    }
  },
)
</script>

<style scoped>
.login-dialog-content {
  padding: 40px 32px;
  background: #131c32;
  border-radius: 12px;
  min-width: 340px;
  display: flex;
  flex-direction: column;
  align-items: center;
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2);
  color: #fff;
}

.login-header {
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.login-header h2 {
  font-size: 26px;
  font-weight: 700;
  margin: 0;
  color: #fff;
  text-align: left;
  line-height: 1.3;
  flex-grow: 1;
}

.login-header .el-button--danger {
  margin-left: 10px;
}

.my-el-form {
  width: 100%;
}
.my-el-form :deep(.el-form-item) {
  margin-bottom: 20px;
}
.my-el-form :deep(.el-input__wrapper) {
  border-radius: 8px;
  background-color: #1a233a;
  box-shadow: none !important;
  border: 1px solid #2b3a5e;
}
.my-el-form :deep(.el-input__inner) {
  color: #e0e0e0;
  &::placeholder {
    color: #a0a0a0;
  }
}
.forgot-password-container {
  text-align: right;
  margin-bottom: 24px;
}
.forgot-password {
  font-size: 14px;
  color: #60a5fa;
  text-decoration: none;
  &:hover {
    text-decoration: underline;
  }
}
.login-btn {
  width: 100%;
  margin-bottom: 16px;
  height: 44px;
  font-size: 16px;
  font-weight: 600;
  border-radius: 8px;
  background-color: #2563eb;
  border-color: #2563eb;
  color: #fff;
}
.login-btn:hover {
  background-color: #1a47a1;
  border-color: #1a47a1;
}
.signup-btn {
  width: 100%;
  height: 44px;
  font-size: 16px;
  font-weight: 600;
  border-radius: 8px;
  background-color: #2a364c;
  border-color: #2a364c;
  color: #e0e0e0;
}
.signup-btn:hover {
  background-color: #3b4a64;
  border-color: #3b4a64;
  color: #fff;
}
.my-el-form :deep(.el-button--info) {
  margin-left: 0 !important;
}
</style>
