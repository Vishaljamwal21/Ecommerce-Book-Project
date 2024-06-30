using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Ecomm_1.DataAccess.Migrations
{
    public partial class AddSP_coverType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE Create_CoverType
	                             @name varchar(50)
                                 AS
	                             insert Covertypes values(@name)");

            migrationBuilder.Sql(@"CREATE PROCEDURE Update_CoverType
                                 @id int,
	                             @name varchar(50)
                                 AS
	                             Update Covertypes set name =@name where id=@id");

            migrationBuilder.Sql(@"CREATE PROCEDURE Delete_CoverType
                                 @id int
                                 AS
	                             delete Covertypes where id=@id");

            migrationBuilder.Sql(@"CREATE PROCEDURE Get_CoverTypes
                                 AS
	                             select * from Covertypes");

            migrationBuilder.Sql(@"CREATE PROCEDURE Get_CoverType
                                 @id int
                                 AS
	                             select* from Covertypes where id=@id");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
