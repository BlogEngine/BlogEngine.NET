module.exports = function (grunt) {
    grunt.initConfig({
        sass: {
            dist: {
                options: {
                    style: 'compact'
                },
                files: {
                    'src/css/styles.css': 'src/scss/styles.scss',
                }
            }
        },
        watch: {
            src: {
                files: ['src/scss/**/*.scss'],
                tasks: ['sass']
            }
        }
    });
    grunt.loadNpmTasks('grunt-contrib-sass');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.registerTask('default', ['watch']);
};
