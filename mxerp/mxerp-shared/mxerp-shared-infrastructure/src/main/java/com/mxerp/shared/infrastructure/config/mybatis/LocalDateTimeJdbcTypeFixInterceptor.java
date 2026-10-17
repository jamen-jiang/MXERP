package com.mxerp.shared.infrastructure.config.mybatis;

import java.lang.reflect.Field;
import java.lang.reflect.Proxy;
import java.sql.PreparedStatement;
import java.time.LocalDateTime;
import java.util.List;
import org.apache.ibatis.executor.parameter.ParameterHandler;
import org.apache.ibatis.mapping.BoundSql;
import org.apache.ibatis.mapping.MappedStatement;
import org.apache.ibatis.mapping.ParameterMapping;
import org.apache.ibatis.plugin.*;
import org.apache.ibatis.reflection.MetaObject;
import org.apache.ibatis.reflection.SystemMetaObject;
import org.apache.ibatis.type.JdbcType;
import org.springframework.stereotype.Component;

@Component
@Intercepts(
    @Signature(
        type = ParameterHandler.class,
        method = "setParameters",
        args = {PreparedStatement.class}))
public class LocalDateTimeJdbcTypeFixInterceptor implements Interceptor {

  @Override
  public Object intercept(Invocation invocation) throws Throwable {
    Object target = unwrapProxy(invocation.getTarget());
    MetaObject handlerMeta = SystemMetaObject.forObject(target);

    MappedStatement ms = (MappedStatement) handlerMeta.getValue("mappedStatement");
    BoundSql boundSql = (BoundSql) handlerMeta.getValue("boundSql");
    Object parameterObject = boundSql.getParameterObject();

    if (parameterObject == null) {
      return invocation.proceed();
    }

    List<ParameterMapping> mappings = boundSql.getParameterMappings();
    if (mappings == null || mappings.isEmpty()) {
      return invocation.proceed();
    }

    MetaObject paramMeta = ms.getConfiguration().newMetaObject(parameterObject);

    for (ParameterMapping pm : mappings) {
      if (pm.getJavaType() != LocalDateTime.class) {
        continue;
      }
      if (pm.getJdbcType() != null) {
        continue;
      }

      Object actualValue;
      try {
        actualValue = paramMeta.getValue(pm.getProperty());
      } catch (Exception e) {
        continue;
      }

      if (actualValue == null || actualValue instanceof LocalDateTime) {
        MetaObject pmMeta = SystemMetaObject.forObject(pm);
        pmMeta.setValue("jdbcType", JdbcType.TIMESTAMP);
      }
    }

    return invocation.proceed();
  }

  private Object unwrapProxy(Object target) {
    if (Proxy.isProxyClass(target.getClass())) {
      Object handler = Proxy.getInvocationHandler(target);
      try {
        Field field = handler.getClass().getDeclaredField("target");
        field.setAccessible(true);
        return unwrapProxy(field.get(handler));
      } catch (Exception e) {
        return target;
      }
    }
    return target;
  }
}
