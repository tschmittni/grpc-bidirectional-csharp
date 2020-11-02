# grpc-bidirectional-csharp

Simple gRPC client and server using bidirectional streaming. The client connects to the backend server through the Apache WebServer http2 proxy. In order to get this example working, Apache needs to have SSL enabled and run on the default port 443. The mod_http2/mod_proxy_http2 modules have to be configured as following:

```
LoadModule http2_module modules/mod_http2.so
LoadModule proxy_module modules/mod_proxy.so
LoadModule proxy_http2_module modules/mod_proxy_http2.so

Protocols h2
SSLProxyEngine On

<Location /tunnel.TunnelMessaging>
    Require all granted
    
    ProxyPass h2://localhost:5001/tunnel.TunnelMessaging retry=0
    ProxyPassReverse https://localhost:5001/tunnel.TunnelMessaging
</Location>
```

After that you can simply start client and server. The client waits 5 seconds before connecting to the server and after waiting another 10 seconds, it sends the first message.
