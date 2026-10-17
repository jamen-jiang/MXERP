package com.mxerp.shared.application.port.ftp;

import lombok.Getter;
import lombok.Setter;
import org.jspecify.annotations.NonNull;

/** FTP 文件 */
@Getter
@Setter
public class FtpFile {

  /** 文件名 */
  @NonNull private String name = "";

  /** 文件大小 */
  private long size;
}
