# Utils-Helpers



Sample log4net.config appender settings
```
  <logger name="position">
    <level value="DEBUG" />
    <appender-ref ref="TESTLogger" />
  </logger>
  <appender name="TESTLogger" type="namespace.AsyncFileAppender">
    <lockingModel type="log4net.Appender.FileAppender+MinimalLock"/>
    <file value="Log\position.txt"/>
    <appendToFile value="true"/>
    <rollingStyle value="Date"/>
    <datePattern value="yyyyMMdd"/>
    <maxSizeRollBackups value="10"/>
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%d{MM.dd.yyyy HH:mm:ss.fff} [%-5level] %logger // %message%newline"/>
    </layout>
    <filter type="log4net.Filter.LevelRangeFilter">
      <levelMin value="ALL"/>
      <acceptOnMatch value="true"/>
    </filter>
    <filter type="log4net.Filter.DenyAllFilter"/>
  </appender>
```
