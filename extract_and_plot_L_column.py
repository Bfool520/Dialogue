#!/usr/bin/env python3
# -*- coding: utf-8 -*-

"""
Extract the 12th column (Excel column 'L') from an Excel file and
generate a boxplot and histogram images.

Usage examples:
  python extract_and_plot_L_column.py
  python extract_and_plot_L_column.py --path "C:\\Users\\6\\Desktop\\数学建模\\C题\\附件.xlsx" --bins 40

Dependencies:
  pip install pandas openpyxl matplotlib
"""

from __future__ import annotations

import argparse
import os
import sys
from typing import Optional, Union

import pandas as pd

# Use a non-GUI backend for headless environments
import matplotlib

matplotlib.use("Agg")
import matplotlib.pyplot as plt  # noqa: E402  (import after backend selection)


def parse_arguments() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description=(
            "Extract Excel column L (12th column) and plot a boxplot and histogram."
        )
    )
    parser.add_argument(
        "--path",
        type=str,
        default=r"C:\\Users\\6\\Desktop\\数学建模\\C题\\附件.xlsx",
        help="Path to the Excel .xlsx file.",
    )
    parser.add_argument(
        "--sheet",
        type=str,
        default=None,
        help=(
            "Sheet name or 0-based index (e.g., '0'). If omitted, uses the first sheet."
        ),
    )
    parser.add_argument(
        "--bins",
        type=int,
        default=30,
        help="Number of bins for the histogram (default: 30).",
    )
    parser.add_argument(
        "--outdir",
        type=str,
        default=".",
        help="Directory to save outputs (PNG images and CSV).",
    )
    return parser.parse_args()


def load_excel(excel_path: str, sheet: Optional[str]) -> pd.DataFrame:
    if sheet is None:
        sheet_name: Union[int, str, None] = 0
    else:
        # Accept numeric strings as 0-based indices
        sheet_name = int(sheet) if sheet.isdigit() else sheet

    try:
        df = pd.read_excel(excel_path, sheet_name=sheet_name, engine="openpyxl")
    except FileNotFoundError as exc:
        raise FileNotFoundError(
            f"未找到 Excel 文件: {excel_path}. 请确认路径是否正确"
        ) from exc
    return df


def extract_l_column(dataframe: pd.DataFrame) -> pd.Series:
    # Ensure the DataFrame has at least 12 columns
    if dataframe.shape[1] < 12:
        raise ValueError(
            f"该表仅有 {dataframe.shape[1]} 列，无法提取第 L 列（第12列）。"
        )
    series_l = dataframe.iloc[:, 11]
    return series_l


def coerce_numeric(series: pd.Series) -> pd.Series:
    numeric_series = pd.to_numeric(series, errors="coerce")
    numeric_series = numeric_series.dropna()
    return numeric_series


def save_series_to_csv(series: pd.Series, out_path: str) -> None:
    series.to_csv(out_path, index=False, header=["L_column_numeric"])


def plot_boxplot(series: pd.Series, out_path: str) -> None:
    fig, ax = plt.subplots(figsize=(6, 4))
    ax.boxplot(series.values, vert=True, patch_artist=True)
    ax.set_title("Boxplot of Column L (12th)")
    ax.set_ylabel("Value")
    plt.tight_layout()
    fig.savefig(out_path, dpi=150)
    plt.close(fig)


def plot_hist(series: pd.Series, out_path: str, bins: int) -> None:
    fig, ax = plt.subplots(figsize=(6, 4))
    ax.hist(series.values, bins=bins, color="#4C72B0", edgecolor="white")
    ax.set_title("Histogram of Column L (12th)")
    ax.set_xlabel("Value")
    ax.set_ylabel("Count")
    plt.tight_layout()
    fig.savefig(out_path, dpi=150)
    plt.close(fig)


def main() -> int:
    args = parse_arguments()
    os.makedirs(args.outdir, exist_ok=True)

    try:
        df = load_excel(args.path, args.sheet)
        series_l_raw = extract_l_column(df)
        series_l = coerce_numeric(series_l_raw)
    except Exception as exc:  # noqa: BLE001
        print(f"读取或处理失败: {exc}")
        return 1

    if series_l.empty:
        print("第 L 列在数值化后为空（可能全为非数值/缺失）。无法绘图。")
        return 2

    # Save numeric values to CSV
    csv_path = os.path.join(args.outdir, "L_column_values.csv")
    save_series_to_csv(series_l, csv_path)

    # Plot and save figures
    boxplot_path = os.path.join(args.outdir, "L_column_boxplot.png")
    hist_path = os.path.join(args.outdir, "L_column_hist.png")
    plot_boxplot(series_l, boxplot_path)
    plot_hist(series_l, hist_path, args.bins)

    # Print brief stats and file outputs
    desc = series_l.describe()
    print("统计描述（数值化后）:")
    print(desc.to_string())
    print(f"已保存: {csv_path}")
    print(f"已保存: {boxplot_path}")
    print(f"已保存: {hist_path}")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())

