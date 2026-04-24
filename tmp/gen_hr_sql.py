import csv
import io
import sys

SRC = "D:\\OneDrive\\\u684c\u9762\\HR_VIEW.csv"
OUT = r"D:\Csharp\ZapLib\tmp\hr_load.sql"

with io.open(SRC, "r", encoding="utf-8-sig") as f:
    rows = list(csv.reader(f))

header = rows[0]
data = [r for r in rows[1:] if r and any(c.strip() for c in r)]

def q(s):
    if s is None or s == "":
        return "NULL"
    return "'" + s.replace("'", "''") + "'"

lines = []
lines.append("SET DEFINE OFF")
lines.append("ALTER SESSION SET NLS_LENGTH_SEMANTICS = 'CHAR';")
lines.append("")
lines.append("BEGIN EXECUTE IMMEDIATE 'DROP VIEW HR_VIEW'; EXCEPTION WHEN OTHERS THEN IF SQLCODE != -942 THEN RAISE; END IF; END;")
lines.append("/")
lines.append("BEGIN EXECUTE IMMEDIATE 'DROP TABLE HR_TABLE PURGE'; EXCEPTION WHEN OTHERS THEN IF SQLCODE != -942 THEN RAISE; END IF; END;")
lines.append("/")
lines.append("")
lines.append("WHENEVER SQLERROR EXIT SQL.SQLCODE")
lines.append("")
lines.append("""CREATE TABLE HR_TABLE (
  EMPN    VARCHAR2(10)  NOT NULL,
  NAME    VARCHAR2(50),
  UNITCD  VARCHAR2(10),
  CUNIT   VARCHAR2(100),
  EUNIT   VARCHAR2(200),
  UNITCD1 VARCHAR2(10),
  CUNIT1  VARCHAR2(100),
  EUNIT1  VARCHAR2(200),
  UNITCD2 VARCHAR2(10),
  CUNIT2  VARCHAR2(100),
  EUNIT2  VARCHAR2(200),
  UNITCD3 VARCHAR2(10),
  CUNIT3  VARCHAR2(100),
  EUNIT3  VARCHAR2(200),
  UNITLVL NUMBER(2),
  EMAIL   VARCHAR2(200),
  CONSTRAINT PK_HR_TABLE PRIMARY KEY (EMPN)
);""")
lines.append("")

cols = ",".join(header)
for r in data:
    while len(r) < len(header):
        r.append("")
    vals = []
    for i, v in enumerate(r):
        col = header[i]
        if col == "UNITLVL":
            vals.append(v if v.strip() else "NULL")
        else:
            vals.append(q(v.strip()))
    lines.append("INSERT INTO HR_TABLE (" + cols + ") VALUES (" + ",".join(vals) + ");")

lines.append("")
lines.append("COMMIT;")
lines.append("")
lines.append("CREATE OR REPLACE VIEW HR_VIEW AS SELECT * FROM HR_TABLE;")
lines.append("")
lines.append("SELECT COUNT(*) AS HR_TABLE_CNT FROM HR_TABLE;")
lines.append("SELECT COUNT(*) AS HR_VIEW_CNT  FROM HR_VIEW;")
lines.append("EXIT;")

with io.open(OUT, "w", encoding="utf-8", newline="\n") as f:
    f.write("\n".join(lines))

print("Wrote {} INSERT lines to {}".format(len(data), OUT))
