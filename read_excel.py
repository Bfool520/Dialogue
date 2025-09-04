#!/usr/bin/env python3
# -*- coding: utf-8 -*-

"""
Read the Excel file at the specified Windows path and print the first rows.

Usage:
  python read_excel.py
  python read_excel.py "C:\\Users\\6\\Desktop\\数学建模\\C题\\附件.xlsx"

Dependencies:
  pip install pandas openpyxl
"""

from __future__ import annotations

import sys
from typing import Optional

import pandas as pd


def read_excel_to_dataframe(excel_path: str) -> pd.DataFrame:
    """Load an Excel file into a pandas DataFrame.

    Args:
        excel_path: Absolute path to the .xlsx file.

    Returns:
        DataFrame containing the first sheet of the workbook.
    """
    # Use openpyxl engine explicitly for .xlsx files
    dataframe: pd.DataFrame = pd.read_excel(excel_path, engine="openpyxl")
    return dataframe


def main() -> int:
    # Default Windows path provided by the user
    default_excel_path: str = r"C:\\Users\\6\\Desktop\\数学建模\\C题\\附件.xlsx"

    # Allow overriding the path via CLI argument
    excel_path: str = sys.argv[1] if len(sys.argv) > 1 else default_excel_path

    try:
        df: pd.DataFrame = read_excel_to_dataframe(excel_path)
    except FileNotFoundError:
        print(f"未找到文件: {excel_path}")
        print("请确认路径是否正确，或将路径作为参数传入该脚本。")
        return 1
    except ImportError:
        print("缺少依赖。请先安装: pip install pandas openpyxl")
        return 1
    except Exception as exc:  # noqa: BLE001 - show any unexpected error clearly
        print(f"读取失败: {exc}")
        return 1

    # Print simple summary and preview
    print(f"读取成功，行数: {len(df)}，列数: {len(df.columns)}")
    try:
        print(df.head(10).to_string(index=False))
    except Exception:
        # Fallback if DataFrame has non-printable cells
        print(df.head(10))

    return 0


if __name__ == "__main__":
    raise SystemExit(main())

