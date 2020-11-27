... Playback setup guide
===

## Source Url
Source Url syntax is defined by [FFmpeg protocols](https://www.ffmpeg.org/ffmpeg-protocols.html). Protocol options, passable as url arguments are not supported. They can be passed to FFmpeg via DataSource Options.

---
## Playback setup
Playback requires an instance of DataPresenter with a specified DataReader and DataProvider.
DataProvider acts as a consumption point for contained DataSources.
DataSource may have protocol options attached to them.

DataPresenter defines playback control API, acting as a connecting layer between Tizen TV platform player and DataProvider. 
    
DataPresenter API offers basic seek, suspend and resume functionality, source allowing.

---
## Example setups
Error handler is optional, albeit, useful. 

Eos handler is used for DataPresenter teardown upon stream completion.

FFmpegPlayer application creates DataPresenter in [EventLoop.cs](../FFmpegPlayer/EventLoop.cs)


### Http / Manifest
```c#
DataPresenter presenter = new EsPlayerPresenter()
    .AddHandlers(OnEos, OnError)
    .With(DataReader.CreateFactoryFor<GenericPacketReader>())
    .With(new SingleSourceDataProvider()
        .Add(new BufferedGenericSource()
            .AddHandler(OnError)
            .Add("http://multiplatform-f.akamaihd.net/i/multi/april11/sintel/sintel-hd_,512x288_450_b,640x360_700_b,768x432_1000_b,1024x576_1400_m,.mp4.csmil/master.m3u8")
            .With(new DataSourceOptions()
                .Set(HttpOption.MultipleRequests,OptionValue.Yes)
                .Set(HttpOption.Reconnect,OptionValue.Yes)
            )));
```


Url can point media content content or manifest file, as long as support for such source is compiled into FFmpeg library.


### Udp
Udp sources, being less common to find around, can be created with FFmpeg command line tool itself


```shell
ffmpeg -re -i 'YourFav.mp4' -f mpegts udp://Tizen TV IP:Port
```

and a code snippet

```c#
DataPresenter presenter = new EsPlayerPresenter()
    .AddHandlers(OnEos, OnError)
    .With(DataReader.CreateFactoryFor<GenericPacketReader>())
    .With(new SingleSourceDataProvider()
        .Add(new BufferedGenericSource()
            .AddHandler(OnError)
            .Add("udp://FFmpeg Source IP:Port")
            .With(new DataSourceOptions()
                .Set(UdpOption.BufferSize, 1024 * 1024)
            )));
```

### Rtsp
RTSP url of choice :metal:


```c#
DataPresenter presenter = new EsPlayerPresenter()
    .AddHandlers(OnEos, OnError)
    .With(DataReader.CreateFactoryFor<GenericPacketReader>())
    .With(new SingleSourceDataProvider()
        .Add(new BufferedGenericSource()
            .AddHandler(OnError)
            .Add("rtsp://Rtsp server IP[:Port]/BearedKinkyDustyContnet")
            .With(new DataSourceOptions()
                .Set(RtspOption.BufferSize, 1024 * 1024)
                .Set(RtspOption.Timeout, 5 * 1000000)
            )));
```

---
## Playback start
Created presenter is started using DataReader.Open() method. DataReader.Open() requires an ElmSharp window to be provided.

---
## Logging
### FFmpegPlayer logs
FFmpegPlayer logs its activity on ffPlay dlog channel.

    Verbose - most detailed activity logs.
    Debug - Debug tracing logs.
    Info - Activity and state logs.

### FFmpeg logs
FFmpeg logs are too available on ffPlay dlog channel, however, FFmpeg log verbosity is not correlated to dlog channel verbosity. All FFmpeg logs are logged at Info level.

FFmpeg log verbosity is application defined in [FFmpegGlue.cs](../Demuxer/FFmpeg/FFmpegGlue.cs)

---
## Remote Control key mapping
FFmpegPlayer application uses following keys:
- Right / Left keys as FF/REW. Seekable content applicable.
- Enter kay as Suspend / Resume.
