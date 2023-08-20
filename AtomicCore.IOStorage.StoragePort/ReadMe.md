nuget引用docker打包编译
docker build -t XXXXXXXXXXXXXX/atomiccore.iostorage.storageport .


直接项目应用后他的docker打包编译

docker build -f "D:\GitHub_Pros\AtomicCore\AtomicCore.IOStorage.StoragePort\Dockerfile" --force-rm -t XXXXXXXXXXXXXX/atomiccore.iostorage.storageport  --label "com.microsoft.created-by=visual-studio" --label "com.microsoft.visual-studio.project-name=AtomicCore.IOStorage.StoragePort" "D:\GitHub_Pros\AtomicCore"


镜像推送至hub.docker.com

docker push XXXXXXXXXXXXXX/atomiccore.iostorage.storageport

外挂存储容器启动

docker pull alpine
docker pull XXXXXXXXXXXXXX/atomiccore.iostorage.storageport

docker run --name alpine-netcore-uploads -it -v uploads:/app/wwwroot/uploads alpine sh

docker run -d -p 8777:80 --name=atomiccore.iostorage.storageport -it --volumes-from alpine-netcore-uploads XXXXXXXXXXXXXX/atomiccore.iostorage.storageport


若为IIS配置，请将对应站点的应用程序池做相关设置
https://www.cnblogs.com/anech/p/16049954.html
找到项目的应用程序池，将高级设置中“加载用户配置文件”（Load User Profile）设置为true即可。