package com.mxerp.shared.application.port.lock;

import java.lang.annotation.Documented;
import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * 分布式锁注解
 *
 * <p><b>注意：目前仅定义了注解本身，尚未实现对应的切面（Redisson 锁拦截器）。</b>在切面补齐之前，标注该注解的方法不会真正加锁。
 *
 * <p>锁 key 由 {@code prefix} + {@code value} 拼成，例如 {@code app:123}。
 */
@Documented
@Target(ElementType.METHOD)
@Retention(RetentionPolicy.RUNTIME)
public @interface DistributedLock {

  /**
   * 锁前缀（一般取 {@code EntityConst} 里的实体名）
   *
   * @return 前缀
   */
  String prefix();

  /**
   * 锁 key 的 SpEL 表达式（相对方法参数求值），如 {@code "#command.id"}
   *
   * @return SpEL 表达式
   */
  String value();
}
