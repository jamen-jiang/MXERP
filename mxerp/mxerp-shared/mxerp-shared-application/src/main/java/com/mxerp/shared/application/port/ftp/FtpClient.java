package com.mxerp.shared.application.port.ftp;

import java.io.InputStream;
import java.util.List;
import org.jspecify.annotations.NonNull;

/** FTP 客户端接口 */
public interface FtpClient {

  /**
   * 获取 FTP 文件内容
   *
   * @param path FTP 文件路径
   * @return 文件内容（可空），文件不存在返回 null
   */
  byte[] get(@NonNull String path);

  /**
   * 将流上传到 FTP 指定路径
   *
   * @param stream 要上传的流
   * @param remotePath FTP 目标路径
   * @return 上传是否成功
   */
  boolean upload(@NonNull InputStream stream, @NonNull String remotePath);

  /**
   * 将字节数组上传到 FTP 指定路径
   *
   * @param buffer 要上传的字节数组
   * @param remotePath FTP 目标路径
   * @return 上传是否成功
   */
  boolean upload(byte[] buffer, @NonNull String remotePath);

  /**
   * 将本地文件上传到 FTP 指定路径
   *
   * @param localPath 本地文件路径
   * @param remotePath FTP 目标路径
   * @return 上传是否成功
   */
  boolean upload(@NonNull String localPath, @NonNull String remotePath);

  /**
   * 删除 FTP 文件
   *
   * @param path FTP 文件路径
   */
  void remove(@NonNull String path);

  /**
   * 创建 FTP 目录
   *
   * @param path FTP 目录路径
   */
  void createDirectory(@NonNull String path);

  /**
   * 判断 FTP 文件是否存在
   *
   * @param path FTP 文件路径
   * @return 文件是否存在
   */
  boolean fileExists(@NonNull String path);

  /**
   * 判断 FTP 目录是否存在
   *
   * @param path FTP 目录路径
   * @return 目录是否存在
   */
  boolean directoryExists(@NonNull String path);

  /**
   * 获取 FTP 目录下的所有文件列表
   *
   * @param path FTP 目录路径
   * @return 文件列表
   */
  @NonNull List<FtpFile> getFtpFiles(@NonNull String path);
}
