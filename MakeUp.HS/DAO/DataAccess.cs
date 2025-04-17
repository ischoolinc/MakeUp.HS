using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FISCA.Data;
using System.Data;

namespace MakeUp.HS.DAO
{
    public class DataAccess
    {
        // 透過班級ID取得學時制學生ID
        public static List<string> GetStudentIDByClassIDs(List<string> classIDlist)
        {
            List<string> value = new List<string>();
            try
            {
                QueryHelper qh = new QueryHelper();
                string sql = string.Format(@"
                SELECT
                    student.id AS student_id
                FROM
                    student
                    INNER JOIN class ON student.ref_class_id = class.id
                WHERE
                    student.status = 1
                     AND class.id IN ({0});
                ", string.Join(",", classIDlist.ToArray()));

                DataTable dt = qh.Select(sql);

                foreach (DataRow dr in dt.Rows)
                {
                    value.Add(dr["student_id"] + "");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return value;
        }

        // 取得學年度課程授課教師id
        public static Dictionary<string, string> GetCourseTeacherIDBySchoolYear(string SchoolYear)
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            try
            {
                string sql = string.Format(@"
            SELECT
                course.ref_class_id,
                course.subject,
                tc_instruct.ref_teacher_id
            FROM
                course
                INNER JOIN tc_instruct ON course.id = tc_instruct.ref_course_id
            WHERE
                course.school_year = {0} 
                AND tc_instruct.sequence = 1
            ORDER BY
                course.semester DESC,
                course.ref_class_id,
                course.subject
            ", SchoolYear);

                QueryHelper qh = new QueryHelper();
                DataTable dt = qh.Select(sql);

                foreach (DataRow dr in dt.Rows)
                {
                    string key = dr["ref_class_id"] + "_" + dr["subject"];
                    if (!value.ContainsKey(key))
                        value.Add(key, dr["ref_teacher_id"] + "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return value;
        }
    }
}
