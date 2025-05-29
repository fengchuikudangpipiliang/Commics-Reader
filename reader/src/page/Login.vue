<template>
  <div class="page-container">
    <el-row :gutter="20">
      <el-col :span="6">
        <div class="grid-content ep-bg-purple">hello</div>
      </el-col>
      <el-col :span="6">
        <div class="grid-content ep-bg-purple">ss</div>
      </el-col>
      <el-col :span="6">
        <div class="grid-content ep-bg-purple">ss</div>
      </el-col>
      <el-col :span="6">
        <div class="grid-content ep-bg-purple">ss</div>
      </el-col>
    </el-row>
    <div class="login-container">
      <div class="left-panel">
        <!-- 背景图片将通过CSS设置 -->
      </div>
      <div class="right-panel">
        <h2>Welcome!<br>Sign in to your Account</h2>
        <form>
          <div class="input-group">
            <input type="text"  v-model="data1" placeholder="Account"/>
          </div>
          <div class="input-group">
            <input type="password"  v-model="data2" placeholder="Password" />
          </div>
          <a href="#" class="forgot-password">Forgot Password?</a>
          <button type="" @click="submitFunc" class="btn primary">SIGN IN</button>
          <button type="button" class="btn secondary">SIGN UP</button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import {ref,watch} from 'vue'
// 组件逻辑.
let data1=ref('')
let data2=ref('')
watch([data1,data2], (newValue) => {
  
});
function submitFunc(){
  const reg = /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$/
  console.log('校验结果：', reg.test(data2.value))
}

</script>

<style scoped>
.page-container {
  padding: 50px;
}

.login-container {
  display: flex;
  width: 100%;
  max-width: 900px;
  margin: 60px auto;
  background: #fff;
  border-radius: 24px;
  box-shadow: 0 4px 32px rgba(0, 0, 0, 0.07);
  overflow: hidden;
  min-height: 600px;
}

.left-panel {
  flex: 1;
  background-image: url('src/resource/imgs/11.jpg');
  background-size: cover;
  background-position: center;
}

.right-panel {
  flex: 1;
  padding: 60px 48px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  min-width: 340px;
}

h2 {
  font-size: 26px;
  font-weight: 600;
  margin-bottom: 18px;
  color: #222;
}

.input-group {
  margin-bottom: 18px;
}

.input-group input {
  width: 100%;
  padding: 11px 16px;
  border: 1.5px solid #dbeafe;
  border-radius: 8px;
  font-size: 15px;
  background: #f8fafc;
  transition: border 0.2s;
}

.input-group input:focus {
  border-color: #60a5fa;
  outline: none;
  background: #fff;
}

.forgot-password {
  font-size: 14px;
  color: #60a5fa;
  margin-bottom: 14px;
  display: block;
  text-align: right;
}

.btn {
  width: 100%;
  padding: 12px;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  cursor: pointer;
  margin-bottom: 12px;
  font-weight: 500;
  transition: background 0.2s;
}

.primary {
  background-color: #2563eb;
  color: #fff;
}

.primary:hover {
  background-color: #1d4ed8;
}

.secondary {
  background-color: #f3f4f6;
  color: #374151;
}

.secondary:hover {
  background-color: #e0e7ef;
}

</style>    