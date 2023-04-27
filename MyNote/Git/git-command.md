### `git init`
创建/初始化本地仓库,生成.git文件夹
### `git config`
 `-l`
 `--system --list`
 系统配置，相关信息存储在文件`安装目录\Git\etc\gitconfig`中
 `--global --list`
 用户配置，相关信息存储在文件`C:\使用者\用户名\.gitconfig`中

 ### 本地仓库

 `git add`

 将当前工作区的所有代码添加到暂存区

 `git add xxx`

 将当前工作区的部分代码添加到暂存区，`xxx`代表文件

  `git diff`

 `git commit -m "xxx"`

提交代码到本地仓库库,`xxx`代表提交说明

 `git commit -a -m "xxx"`

跳过`git add`暂存区操作，直接提交代码到本地仓库库,`xxx`代表提交说明

 ### 远程仓库
 ``