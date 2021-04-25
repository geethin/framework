# 说明
高新区供应链平台后台接口服务。


# 规范

## 关于文档
- 作者：NilTor
- 在线文档: [开发环境](http://140.249.184.1:8090/api/docs)。

## 关于请求
请求使用的Http方式如下：
- GET:简单查询，如详情、无筛选分页的列表等
- POST:新增数据；筛选查询。除上传文件外，仅支持json格式。
- PUT:更新数据；筛选查询。除上传文件外，仅支持json格式。
- DELETE:删除数据。
- 
## 关于返回
请先根据http返回的状态码进行判断，遵循http协议规范。
- 200 返回成功
- 201 新增成功
- 400 请求错误
- 404 未找到
- 401 未授权
- 403 禁止访问
- 409 冲突，比如尝试新增重复的内容
- 50x 业务处理失败或服务器报错 

对于新增、编辑、删除、查询等接口，会直接返回（状态码为200时）所需要的数据，请以文档中的返回结构为准。

对于400请求错误返回（通常为参数校验），格式如下:
```json
{
  "errors": {
    "Captcha": [
      "The 验证码 field is required."
    ],
    "Password": [
      "The 密码 field is required.",
      "The field 密码 must be a string or array type with a minimum length of '6'."
    ],
    "Username": [
      "The 用户名 field is required.",
      "The field 用户名 must be a string or array type with a minimum length of '6'."
    ]
  },
  "type": "",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-eb8d441a86f29b43a475779322e08d9c-1d014f80f0f0f84c-00"
}
```

对于其他业务逻辑错误信息，状态码为500。格式如下:
```json
{
    "type": "error",
    "title": "业务逻辑错误",
    "status": 500,
    "detail": "提示信息",
    "traceId": "00-06b34e1ff7f0f74ba96e4810da86e3f8-34d5363bf7502940-00"
}
```

如果是服务端系统异常，程序错误，状态码为500，其中type为`Server Error`：
```json
{
    "type": "Server Error",
    "title": "",
    "status": 500,
    "detail": "异常信息"
}
```