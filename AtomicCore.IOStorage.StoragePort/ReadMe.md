nuget����docker�������
docker build -t XXXXXXXXXXXXXX/atomiccore.iostorage.storageport .


ֱ����ĿӦ�ú�����docker�������

docker build -f "D:\GitHub_Pros\AtomicCore\AtomicCore.IOStorage.StoragePort\Dockerfile" --force-rm -t XXXXXXXXXXXXXX/atomiccore.iostorage.storageport  --label "com.microsoft.created-by=visual-studio" --label "com.microsoft.visual-studio.project-name=AtomicCore.IOStorage.StoragePort" "D:\GitHub_Pros\AtomicCore"


����������hub.docker.com

docker push XXXXXXXXXXXXXX/atomiccore.iostorage.storageport

��Ҵ洢��������

docker pull alpine
docker pull XXXXXXXXXXXXXX/atomiccore.iostorage.storageport

docker run --name alpine-netcore-uploads -it -v uploads:/app/wwwroot/uploads alpine sh

docker run -d -p 8777:80 --name=atomiccore.iostorage.storageport -it --volumes-from alpine-netcore-uploads XXXXXXXXXXXXXX/atomiccore.iostorage.storageport


��ΪIIS���ã��뽫��Ӧվ���Ӧ�ó�������������
https://www.cnblogs.com/anech/p/16049954.html
�ҵ���Ŀ��Ӧ�ó���أ����߼������С������û������ļ�����Load User Profile������Ϊtrue���ɡ�