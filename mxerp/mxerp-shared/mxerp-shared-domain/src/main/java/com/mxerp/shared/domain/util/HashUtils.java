package com.mxerp.shared.domain.util;

import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import org.jspecify.annotations.Nullable;

/** 哈希工具 */
public final class HashUtils {

  private HashUtils() {}

  /**
   * 计算字符串的 MD5 哈希值
   *
   * @param input 输入字符串
   * @return 小写十六进制 MD5，输入为空返回空字符串
   */
  public static String computeMd5(@Nullable String input) {
    if (input == null || input.isEmpty()) {
      return "";
    }
    try {
      MessageDigest md = MessageDigest.getInstance("MD5");
      byte[] hashBytes = md.digest(input.getBytes(StandardCharsets.UTF_8));
      StringBuilder sb = new StringBuilder(hashBytes.length * 2);
      for (byte b : hashBytes) {
        sb.append(Character.forDigit((b >> 4) & 0xF, 16));
        sb.append(Character.forDigit(b & 0xF, 16));
      }
      return sb.toString();
    } catch (NoSuchAlgorithmException e) {
      // JDK 必然支持 MD5，不可能走到这里
      throw new IllegalStateException("MD5 algorithm not available", e);
    }
  }
}
