# Building and Running with Docker 

Currently, they are no images pushed to DockerHub, so you have to build the image yourself for example with:

```
docker build . -t textrude:dev
```

For using the newly built image you can use this command:

```
docker run -v ${PWD}:/work textrude:dev render --models /work/examples/hello.json --template /work/examples/hello.sbn
```